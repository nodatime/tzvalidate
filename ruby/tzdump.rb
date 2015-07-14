#!/usr/bin/env ruby

require 'tzinfo'

# Constants

START_UTC = DateTime.new(1905, 1, 1, 0, 0, 0, '+0')
END_UTC = DateTime.new(2035, 1, 1, 0, 0, 0, '+0')

def format_offset(offset)
  sign = offset < 0 ? "-" : "+"
  offset = offset.abs
  return '%s%02d:%02d:%02d' % [sign, offset / 3600, (offset / 60) % 60, offset % 60]
end

def dump_zone(id)
  $stdout.write "#{id}\r\n"
  zone = TZInfo::Timezone.get(id)
  period = zone.period_for_utc(START_UTC)
  if (period.end_transition.nil? || period.end_transition.at > END_UTC)
    $stdout.write "Fixed: #{format_offset(period.offset.utc_total_offset)} #{period.offset.abbreviation}\r\n"
  else
    while !period.end_transition.nil? && period.end_transition.at < END_UTC
      period = zone.period_for_utc(period.end_transition.at)
      start = period.start_transition.at.to_datetime
      formatted_start = start.strftime("%Y-%m-%dT%H:%M:%SZ")
      formatted_offset = format_offset(period.offset.utc_total_offset)
      $stdout.write "#{formatted_start} #{formatted_offset} #{period.dst? ? "daylight" : "standard"} #{period.offset.abbreviation}\r\n"
    end
  end
end


tzs = if ARGV.empty?
        TZInfo::Timezone.all_identifiers.sort
      else
        ARGV[0]
      end

tzs.each do |id|
  dump_zone(id)
  $stdout.write "\r\n"
end
