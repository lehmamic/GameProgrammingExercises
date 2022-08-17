// See https://aka.ms/new-console-template for more information

//Create a window.

using GameProgrammingExercises;
using Silk.NET.Windowing;

using var game = new Game();

game.Initialize()
    .Run();



// private static void OnRender(double obj)
// {
//     //Here all rendering should be done.
// }
//
// private static void OnUpdate(double obj)
// {
//     //Here all updates to the program should be done.
// }
//
// private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
// {
//     //Check to close the window on escape.
//     if (arg2 == Key.Escape)
//     {
//         window.Close();
//     }
// }