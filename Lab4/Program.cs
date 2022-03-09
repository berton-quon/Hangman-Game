using System;
using System.Drawing;
using GDIDrawer;
using System.IO;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            string secret = "";             //temporary placeholder of hangman word - to be changed after reading txt file
            int wrong;                      //counter for wrong guesses
            char guess;                     //current letter guess
            string guessed;                 //string of guessed letters
            int success;                    //counter for successful guesses
            int index;                      //index counter to assign char of hangman word to char array
            char[] word;                    //char array of the hangman word
            string rerun;                   //string to rerun program          
            string displayed;               //current string of "-" and correctly guessed letters

            do
            {
                //reset variables and clear console
                Console.Clear();
                wrong = 0;
                guessed = "";
                success = 0;
                displayed = "";

                //name and title
                Console.WriteLine("\t\t\t\tBerton Quon - Lab 4");

                //load file with strings
                StreamReader wordFile;                              
                wordFile = new StreamReader("words.txt");
                secret = LoadString(ref wordFile);                  //obtain new word for game from txt file
                word = new char[secret.Length];                     //create new char array equal to length of word
                for (index = 0; index < secret.Length; index++)     //assign dashes to each char in hangman word
                {
                    word[index] = char.Parse("-");
                    displayed = displayed + word[index];
                }
            
                //create GDI drawer window and background image of hangman
                CDrawer canvas = new CDrawer();
                DrawScreen(ref canvas, ref guessed, word, ref displayed);

                //while loop until success or fail of game
                do
                {
                    //ask user to enter a letter
                    GetGuess(ref canvas, out guess, ref guessed);

                    //check to ensure valid letter and not previously played
                    CheckGuess(ref guess, ref guessed, ref wrong, ref secret, word, ref success, ref displayed);
                    canvas.Clear();

                    //add to hangman drawing if wrong, redisplay most updated game
                    DrawScreen(ref canvas, ref guessed, word, ref displayed);
                    WrongGuess(ref canvas, ref wrong, secret);

                } while ((wrong < 6) && (success < secret.Length));     //continue asking for letters until game lost or won

                if (success == secret.Length)       //if win
                    canvas.AddText("You Win!", 80, Color.Green);
                
                //ask user to rerun program
                Console.Write("\nWould you like to play again? Yes/No: ");
                rerun = Console.ReadLine().ToLower();
                if (rerun == "yes")
                    canvas.Close();      //close canvas
                
            } while (rerun == "yes");       //replay hangman
        }

        static private string LoadString(ref StreamReader wordFile)
        {
            int read = 0;                           //counter for loop  
            string findWord = "";                   //hangman word
            Random value = new Random();
            int randomValue = value.Next(5459);     //random number generator to select a line to read from and choose word

            try
            {
                try
                {
                    for (read = 0; read <= randomValue; read++)
                    {
                        wordFile.ReadLine();
                        if (read == randomValue)
                            findWord = wordFile.ReadLine();             //assign word to game
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine($"Error reading file: {e.Message}");      //catch exceptions for reading file
                }
                finally
                {
                    wordFile.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error opening file: {e.Message}");          //catch exceptions for opening file 
            }
            return findWord;            //return word to main program for game 
        }

        static private void DrawScreen(ref CDrawer canvas, ref string guessed, char[] word, ref string displayed)
        {
            //add background image
            canvas.AddLine(300, 100, 300, 450, Color.Aqua, 5);
            canvas.AddLine(300, 125, 450, 125, Color.Aqua, 5);
            canvas.AddLine(300, 150, 325, 125, Color.Aqua, 5);
            canvas.AddLine(300, 400, 500, 400, Color.Aqua, 5);
            canvas.AddLine(300, 410, 475, 440, Color.Aqua, 5);
            canvas.AddLine(300, 440, 475, 410, Color.Aqua, 5);
            canvas.AddLine(475, 400, 475, 450, Color.Aqua, 5);
            canvas.AddLine(400, 125, 400, 175, Color.Brown, 3);
            //show most updated letters used and current progress on hangman word
            canvas.AddText($"Letters used: {guessed}", 20, 180, 50, 400, 50, Color.White);          //display letters used 
            canvas.AddText($"{displayed}", 50, 0, 250, 800, 500, Color.Yellow);               //display current progess on hangman word
        }

        static private void GetGuess(ref CDrawer canvas, out char guess, ref string guessed)
        {
            bool retry;                 //bool to rerun method if letter has already been previously picked
            string input;               //temporary keyboard input storage
            
            do
            {
                retry = false;

                Console.Write("Guess a letter: ");
                input = Console.ReadLine();
                char.TryParse(input, out guess); //ensure only a single character was inputted
                while (!char.IsLetter(guess))    //check if input was a letter
                {
                    Console.WriteLine("The character you entered is not a letter.");
                    Console.Write("\nGuess a letter: ");
                    input = Console.ReadLine();
                    char.TryParse(input, out guess);
                }
                foreach (char c in guessed)     //check if letter has already been guessed
                {
                    if (c == guess)
                    {
                        Console.WriteLine("The letter you have entered has already been selected.");
                        retry = true;
                    }
                }
            }
            while (retry);           //loop request for letter until valid, new letter is entered 
            guessed = guessed + guess;          //update list of letters used
           
        }
        static private void CheckGuess(ref char guess, ref string guessed, ref int wrong, ref string secret, char[] word, ref int success, ref string displayed)
        {
            bool found = false;     //bool indicating letter is found
            int index = 0;          //index to change displayed characters
            int wordIndex;          //index to display hangman word
            foreach (char s in secret)
            {
                if (s == guess)
                {
                    //display letter in word
                    word[index] = s;            //assign correct letter to char[]
                    found = true;
                    success++;                  //add to success counter (determines when to end game)
                }
                index++;
            }
            if (!found)                         //letter guessed is incorrect 
            {
                Console.WriteLine($"'{guess}' is not in the word.");
                wrong++; //add to wrong counter - determines what part of hangman to draw
            }

            displayed = "";
            for (wordIndex = 0; wordIndex < secret.Length; wordIndex++)
            {
                displayed = displayed + word[wordIndex];                //adjust string to show correctly guessed letters 
            }
            if (wrong == 6)
                displayed = secret;
            Console.WriteLine();

        }
        static private void WrongGuess(ref CDrawer canvas, ref int wrong, string secret)
        {
            //draw appropriate part of hangman depending on how many wrong letters have been chosen 
            if (wrong >= 1)
                canvas.AddCenteredEllipse(400, 185, 20, 20, Color.Yellow);      //head
            if (wrong >= 2)
                canvas.AddCenteredEllipse(400, 230, 20, 70, Color.Yellow);      //body
            if (wrong >= 3)
                canvas.AddLine(390, 210, 370, 230, Color.Yellow, 3);            //arm
            if (wrong >= 4)
                canvas.AddLine(410, 210, 430, 230, Color.Yellow, 3);            //arm
            if (wrong >= 5)
                canvas.AddLine(390, 250, 380, 290, Color.Yellow, 3);            //leg
            if (wrong >= 6)
            {
                canvas.AddLine(410, 250, 420, 290, Color.Yellow, 3);            //leg
                canvas.AddText("You Lose!", 80, Color.Blue);                    //display loss
                Console.WriteLine("You lose.");
            }
        }
    }
}
