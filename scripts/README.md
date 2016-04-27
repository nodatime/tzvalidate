Useful scripts for converting data. These are *very* ad hoc,
and should only be seen as a starting point for more robust
automation. They work on my machine...

compile.sh
----

Assumes a directory hierarchy of:

    (some root)
       \- input
          \- tzdata2015a
          \- tzdata2015b
          \- etc
       \- output

If you're in the `input` directory, you can run

    ./compile.sh tzdata2015a
    
... to create `tzdata2015a` in the `output` directory.

generate.sh
----

Used by CI, this requires .NET Core to be installed, but is otherwise
reasonably portable.

Example:

    ./generate 2016d
    
... fetches both code and data for 2016d, builds `zic` and runs it,
then builds and runs the `ZicDump` code to generate the validation
text file. The result is in `tmp` relative to the root project directory.