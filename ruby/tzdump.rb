# Note: this currently uses the system line break, instead of specifying CR-LF.
# I'm not sure how to specify the line break when running Ruby...

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
  puts "#{id}"
  zone = TZInfo::Timezone.get(id)
  period = zone.period_for_utc(START_UTC)
  if (period.end_transition.nil? || period.end_transition.at > END_UTC)
    puts "Fixed: #{format_offset(period.offset.utc_total_offset)} #{period.offset.abbreviation}"
  else
    while !period.end_transition.nil? && period.end_transition.at < END_UTC
      period = zone.period_for_utc(period.end_transition.at)
      start = period.start_transition.at.to_datetime
      formatted_start = start.strftime("%Y-%m-%dT%H:%M:%SZ")
      formatted_offset = format_offset(period.offset.utc_total_offset)
      puts "#{formatted_start} #{formatted_offset} #{period.dst? ? "daylight" : "standard"} #{period.offset.abbreviation}"
    end
  end
end


if ARGV.empty?
  for id in TZInfo::Timezone.all_identifiers.sort
    dump_zone(id)
    puts()
  end
else
  dump_zone(ARGV[0])
end
