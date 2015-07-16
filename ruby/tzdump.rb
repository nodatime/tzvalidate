#!/usr/bin/env ruby

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

  def self.console
    @console ||= IO.new($stdout.fileno, mode: "w", universal_newline: true)
  end

  def self.puts(*messages)
    console.write(messages.join("\r\n"))
    console.write("\r\n")
  end
end

# Functions

def format_offset(offset)
  sign = offset < 0 ? "-" : "+"
  offset = offset.abs

  "%s%02d:%02d:%02d" % [sign, offset / 3600, (offset / 60) % 60, offset % 60]
end

def dump_period(period, zone)
  # TODO: I feel like this could be moved to some iterator, but that would mean refactoring into classes:
  while !period.end_transition.nil? && period.end_transition.at < END_UTC
    period = zone.period_for_utc(period.end_transition.at)
    start = period.start_transition.at.to_datetime
    formatted_start = start.strftime("%Y-%m-%dT%H:%M:%SZ")
    formatted_offset = format_offset(period.offset.utc_total_offset)

    CrLf.puts "#{formatted_start} #{formatted_offset} #{period.dst? ? "daylight" : "standard"} #{period.offset.abbreviation}"
  end
end

def dump_zone(id)
  CrLf.puts "#{id}"
  zone = TZInfo::Timezone.get(id)
  period = zone.period_for_utc(START_UTC)

  if period.end_transition.nil? || period.end_transition.at > END_UTC
    CrLf.puts "Fixed: #{format_offset(period.offset.utc_total_offset)} #{period.offset.abbreviation}"
  else
    dump_period(period, zone)
  end

  CrLf.puts
end

# Main

# TODO: This would probably benefit from using Thor, instead:
tzs = ARGV.empty? ? TZInfo::Timezone.all_identifiers.sort : ARGV[0]
tzs.each {|id| dump_zone(id) }
