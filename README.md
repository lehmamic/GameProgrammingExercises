# Game Programming Exercises

C# port of the book Game Programming in C++. [Original Code](https://github.com/gameprogcpp/code)

## Preparations

### FMOD

The exercises uses the FMOD sound engine. Download the **FMOD Studio API** binaries for your OS (MAC/Windows) into the `External/FMOD/FMOD Programmers API` folder.

## Problems which needed to be solved

### Exercise 6

- C# Matrices have a row-major memory layout. When setting the matrix uniforms in the OpenGL shader, a special transpose flag needs to be set to true in order that OpenGL converts them correctly. Then we can use normal Row Vector multiplication in the shader.
- The C# Quaternions are instantiated differently than in the book. That's why I used a special factory methods to create them.
- The perspective and view matrices in c# uses a right han coordinate system, the book uses a left hand coordinate system. Thats why I migrated the code from the book directly to C# as well.
