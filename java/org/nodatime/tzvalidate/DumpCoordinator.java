// Copyright 2016 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.StringWriter;
import java.io.Writer;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import org.apache.commons.cli.ParseException;

public class DumpCoordinator {

    public static void dump(ZoneTransitionsProvider provider, boolean supportsSource, String[] args) throws ParseException, IOException {
        DumpOptions options = DumpOptions.parse(provider.getClass().getSimpleName(), args, supportsSource);
        if (options == null) {
            return;
        }
        provider.initialize(options);

        List<String> zoneIds = new ArrayList<>();
        // Urgh... not quite worth using Guava just for Iterables... 
        for (String id : provider.getZoneIds()) {
            zoneIds.add(id);
        }
        Collections.sort(zoneIds);
        
        int fromYear = options.getFromYear();
        int toYear = options.getToYear();
        String zoneId = options.getZoneId();
        boolean includeAbbreviations = options.includeAbbreviations();
        
        // Single zone dump
        if (zoneId != null) {
            if (!zoneIds.contains(zoneId)) {
                System.out.println("Zone " + zoneId + " does not exist");
                return;
            }
            try (Writer writer = new OutputStreamWriter(System.out, StandardCharsets.UTF_8)) {
                provider.getTransitions(zoneId, fromYear, toYear).writeTo(writer, includeAbbreviations);
            }
        }
        
        // All zones, with headers.
        Writer bodyWriter = new StringWriter();
        for (String id : zoneIds) {
            provider.getTransitions(id, fromYear, toYear).writeTo(bodyWriter, includeAbbreviations);
            bodyWriter.write("\n");
        }
        bodyWriter.flush();
        String text = bodyWriter.toString();
        try (Writer overallWriter = new OutputStreamWriter(System.out, StandardCharsets.UTF_8)) {
            writeHeaders(text, provider, options, overallWriter);
            overallWriter.write(text);
        }
    }
    
    private static void writeHeaders(String text, ZoneTransitionsProvider dumper, DumpOptions options, Writer writer)
        throws IOException
    {
        String version = options.getVersion();
        if (version != null)
        {
            writer.write("Version: " + version + "\n");
        }
        
        MessageDigest sha256Digest;
        try {
            sha256Digest = MessageDigest.getInstance("SHA-256");
        } catch (NoSuchAlgorithmException e) {
            throw new RuntimeException("No SHA-256 available");
        }
        sha256Digest.update(text.getBytes(StandardCharsets.UTF_8));
        byte[] hash = sha256Digest.digest();
        writer.write("Body-SHA-256: ");
        String hexDigits = "0123456789abcdef";
        for (byte b : hash) {
            writer.write(hexDigits.charAt((b >> 4) & 0xf));
            writer.write(hexDigits.charAt(b & 0xf));
        }
        writer.write("\n");
        writer.write("Format: tzvalidate-0.1\n");
        writer.write("Range: " + options.getFromYear() + "-" + options.getToYear() + "\n");
        writer.write("Generator: " + dumper.getClass().getName() + "\n");
        writer.write("GeneratorUrl: https://github.com/nodatime/tzvalidate\n");
        writer.write("\n");
    }
}
