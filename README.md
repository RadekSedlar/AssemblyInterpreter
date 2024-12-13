# Assembly interpreter.

This is small passion project to create easy assembly language, with single pass interpreter written in C#.
Language will have 2 main sections marked with `.data` and `.text` respectively.
Language is HEAVILY inspired by [paper](https://www.cs.dartmouth.edu/~sergey/cs258/tiny-guide-to-x86-assembly.pdf) written by Adam Ferrari and Mike Lack.


## Comments
Syntax of comments is same throughout sections. Character `;` is used as comment start and new line character `\n` marks end of the comment.


## `.data` section
This section of program is used for declaring global variables with initial values.

### Grammar
```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```


# TODO
1. finish data interpreter
   1. Create DB (data byte - 8bits) variable
   2. Create DW (data word - 16bits) variable
   3. Create DD (data doubleWord - 33bits) variable