#!/usr/bin/env ruby

## TODO:  Add some TL doc's!

# To avoid having to run with `bundle exec $0`
require "rubygems"
require "bundler/setup"

require "tzinfo"

# Constants

START_UTC = DateTime.new(1905, 1, 1, 0, 0, 0, "+0")
END_UTC = DateTime.new(2035, 1, 1, 0, 0, 0, "+0")

# This is a simple wrapper for $stdout.write in-order to force CRLF line-endings for STOOUT
module CrLf
  attr_writer :console
  # Over-engineered, perhaps:
  def self.console
    @console ||= IO.new($stdout.fileno, mode: "w", universal_newline: true)
  end

  def self.puts(*messages)
    console.write(messages.join("\r\n"))
    console.write("\r\n")
  end
end

# This is a dumper class for the TZInfo data
class Zone
  attr_reader :tz_id

  def initialize(tz_id)
    @tz_id = tz_id
  end

  def zone
    @zone ||= TZInfo::Timezone.get(tz_id)
  end

  def period
    @period ||= zone.period_for_utc(START_UTC)
  end

  def each
    return enum_for(:each) unless block_given?

    p = period
    yield p = zone.period_for_utc(p.end_transition.at) while !p.end_transition.nil? && p.end_transition.at < END_UTC
  end

  def dump
    CrLf.puts "#{tz_id}"

    if period.end_transition.nil? || period.end_transition.at > END_UTC
      CrLf.puts "Fixed: #{format_offset(period.offset.utc_total_offset)} #{period.offset.abbreviation}"

    else
      each do |p|
        formatted_start = p.start_transition.at.to_datetime.strftime("%Y-%m-%dT%H:%M:%SZ")
        formatted_offset = format_offset(p.offset.utc_total_offset)

        CrLf.puts "#{formatted_start} #{formatted_offset} #{p.dst? ? 'daylight' : 'standard'} #{p.offset.abbreviation}"
      end
    end

    CrLf.puts
  end

  private

  def format_offset(offset)
    sign = offset < 0 ? "-" : "+"
    offset = offset.abs

    "%s%02d:%02d:%02d".format(sign, offset / 3600, (offset / 60) % 60, offset % 60)
  end
end

# Main

# TODO: This would probably benefit from using Thor, instead:
tzs = ARGV.empty? ? TZInfo::Timezone.all_identifiers.sort : ARGV[0]

tzs.each {|tz_id| Zone.new(tz_id).dump }
