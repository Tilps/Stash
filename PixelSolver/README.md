# Pixel Solver

A simple program to solve puzzles which are described by numbers of consecutive same color for each row and column.

Iteratively progresses row and column at a time, does not create or propogate any inferences other than the known color
of a given square.  This is good enough to solve almost all published puzzles of this type, but not all puzzles.

Has a little internal quirk - rather than determining just whether a given cell is a given color it counts the
number of ways a row or column can be solved for each option.  In simple black/white puzzles this could then be
used to give a gray scale shading indicating the relative number of scenarios for each option.  Such a feature 
is not implemented.