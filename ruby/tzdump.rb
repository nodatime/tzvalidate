require 'trollop'
require 'tzinfo'

# Constants

START_UTC = DateTime.new(1905, 1, 1, 0, 0, 0, '+0')
END_UTC = DateTime.new(2035, 1, 1, 0, 0, 0, '+0')

# Formats the offset, standard/daylight and abbreviation of a period
def format_period(period)
  offset = period.offset.utc_total_offset
  sign = offset < 0 ? "-" : "+"
  offset = offset.abs
  formatted_offset = '%s%02d:%02d:%02d' % [sign, offset / 3600, (offset / 60) % 60, offset % 60]
  return '%s %s %s' % [formatted_offset, period.dst? ? "daylight" : "standard", period.offset.abbreviation]
end

def dump_zone(console, id, options)
  console.write "#{id}\r\n"
  start_utc = DateTime.new(options.from_year, 1, 1, 0, 0, 0, '+0')
  end_utc = DateTime.new(options.to_year, 1, 1, 0, 0, 0, '+0')
  zone = TZInfo::Timezone.get(id)

  # Take 1AD as "far back in history"
  initial = DateTime.new(1, 1, 1, 0, 0, 0, '+0')
  console.write "Initially: #{format_period(zone.period_for_utc(initial))}\r\n"

  # We want to use the start of periods, so we find the period containing start_utc,
  # then the period which starts at the end of that one. That way we find the first
  # transition after start_utc.
  period = zone.period_for_utc(start_utc)
  if period.end_transition.nil?
    return
  end
  period = zone.period_for_utc(period.end_transition.at)
  while !period.start_transition.nil? && period.start_transition.at < end_utc
    formatted_start = period.start_transition.at.to_datetime.strftime("%Y-%m-%dT%H:%M:%SZ")
    formatted_period = format_period(period)
    console.write "#{formatted_start} #{formatted_period}\r\n"
    # Handle hitting the final transition in the zone 
    if period.end_transition.nil?
      break
    end
    period = zone.period_for_utc(period.end_transition.at)
  end
end

# It's surprisingly hard to persuade Ruby that we know the line endings
# we want, regardless of the host platform.
console = IO.new(STDOUT.fileno, mode: 'w', universal_newline: true)

options = Trollop::options do
  opt :from_year, "Lower bound (inclusive) to print transitions from.", :type => :int, :default => 1
  opt :to_year, "Upper bound (inclusive) to print transitions from.", :type => :int, :default => 2035
  opt :zone, "Single zone ID to dump.", :type => :string
end

if options.zone.nil?
  for id in TZInfo::Timezone.all_identifiers.sort
    dump_zone(console, id, options)
    console.write "\r\n"
  end
else
  dump_zone(console, options.zone, options)
end
