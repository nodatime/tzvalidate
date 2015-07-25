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
