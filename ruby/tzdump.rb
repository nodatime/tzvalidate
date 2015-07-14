#!/usr/bin/env ruby

require 'tzinfo'

# Constants

START_UTC = DateTime.new(1905, 1, 1, 0, 0, 0, '+0')
END_UTC = DateTime.new(2035, 1, 1, 0, 0, 0, '+0')

def puts_crlf(*messages)
  $stdout.write(messages.join("\r\n"))
  $stdout.write("\r\n")
end

def format_offset(offset)
  sign = offset < 0 ? "-" : "+"
  offset = offset.abs
  return '%s%02d:%02d:%02d' % [sign, offset / 3600, (offset / 60) % 60, offset % 60]
end

def dump_period(period, zone)
  # TODO: I feel like this could be moved to some iterator, but it's fairly readable as-is:
  while !period.end_transition.nil? && period.end_transition.at < END_UTC
    period = zone.period_for_utc(period.end_transition.at)
    start = period.start_transition.at.to_datetime
    formatted_start = start.strftime("%Y-%m-%dT%H:%M:%SZ")
    formatted_offset = format_offset(period.offset.utc_total_offset)
    puts_crlf "#{formatted_start} #{formatted_offset} #{period.dst? ? "daylight" : "standard"} #{period.offset.abbreviation}"
  end
end

def dump_zone(id)
  puts_crlf "#{id}"
  zone = TZInfo::Timezone.get(id)
  period = zone.period_for_utc(START_UTC)

  if (period.end_transition.nil? || period.end_transition.at > END_UTC)
    puts_crlf "Fixed: #{format_offset(period.offset.utc_total_offset)} #{period.offset.abbreviation}"
  else
    dump_period(period, zone)
  end
end

# Main

tzs = if ARGV.empty?
        TZInfo::Timezone.all_identifiers.sort
      else
        ARGV[0]
      end

tzs.each do |id|
  dump_zone(id)
  puts_crlf
end
