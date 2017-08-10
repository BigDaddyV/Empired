using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GuessTheAnimal
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the animal data
            JArray animalArray = GetApplicationData();
          
            // ------------------------------------------- Display initial screen ---------------------------------------------
            Console.WriteLine("===================================================================");
            Console.WriteLine("================== WELCOME TO GUESS THE ANIMAL! ===================");
            Console.WriteLine("===================================================================");
            Console.WriteLine();
            Console.WriteLine("Choose one of the animals below and keep your choice in your head:");
            Console.WriteLine();
            foreach (var animalObject in animalArray)
            {
                Console.WriteLine(animalObject["species"]);
            }
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine();
            bool match = false;
            bool finish = false;
            bool loopAgain = false;
            String[] attributes = { "colour", "sound", "feature" };

            // ----------------------- Ask the user questions in order to try and guess the animal ----------------------------
            foreach (var attribute in attributes)
            {      
                while(true)
                {
                    foreach (var animalObject in animalArray.ToList())
                    {
                        match = AskQuestion(attribute, animalObject);
                        if (match == false)
                        {
                            // Remove animals from the animal array that have an attribute value that the user says the animal doesn't have                       
                            RemoveMatchingAnimals(attribute, animalObject[attribute].ToString(), animalArray);
                            if (animalArray.Count < 2)
                            {
                                loopAgain = false;                              
                            } else
                            {
                                loopAgain = true;                               
                            }
                        }
                        else
                        {
                            // Remove other animals that lack the current confirmed attribute value
                            RemoveNonMatchingAnimals(attribute, animalObject[attribute].ToString(), animalArray);
                            loopAgain = false;                          
                        }
                        break;
                    }
                    if (loopAgain == false)
                    {
                        break;
                    }
                }                                    
                              
                finish = GuessTheAnimal(animalArray);
                if (finish == true)
                {
                    break;
                }
            }
                           
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine();

            // -------------------------------- Ask user whether they want to add their own animal ----------------------------            

            animalArray = GetApplicationData(); // Refresh data
            string readChar = "";
            string readCharLower = "";
            do
            {              
                Console.WriteLine("Would you like to add your own animal to our collection ? Yes (y) or No (n)");
                readChar = Console.ReadLine();
                readCharLower = readChar?.ToLower();
                if ((readCharLower == "y") || (readCharLower == "n"))
                    break;
            } while (true);

            string species = "";
            string colour = "";
            string sound = "";
            string feature = "";
            if (readCharLower == "y")
            {
                Console.WriteLine("What species is this new animal?");
                species = Console.ReadLine();

                // Check that the species doesnt already exist
                IList<JToken> sameSpecies = animalArray.Select(m => (JToken)m).Where(n => n["species"].ToString().ToLower() == species.ToLower()).ToArray();
                if (sameSpecies.Count > 0)
                {
                    Console.WriteLine("Sorry, " + species + " already exists");                                      
                } else
                {
                    Console.WriteLine("What colour is this new animal?");
                    colour = Console.ReadLine();

                    Console.WriteLine("What sound does this new animal make?");
                    sound = Console.ReadLine();

                    Console.WriteLine("What is a key feature of this new animal?");
                    feature = Console.ReadLine();

                    // Create new JSON Animal Object and add it to the AnimalArray                                                      
                    JObject newAnimal = new JObject();
                    newAnimal["species"] = species;
                    newAnimal["colour"] = colour;
                    newAnimal["sound"] = sound;
                    newAnimal["feature"] = feature;
                    animalArray.Add(newAnimal);

                    // Update the JSON file
                    UpdateApplicationData(animalArray);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Goodbye - See you next time");
            Console.ReadKey();         
        }

        /// <summary>
        /// This method removes any animal that has a certain attribute value
        /// </summary>
        /// <param name="typeOfQuestion"></param>
        /// <param name="value"></param>
        /// <param name="animalArray"></param>
        private static void RemoveMatchingAnimals(String typeOfQuestion, String value, JArray animalArray)
        {
            IList<JToken> animalsToRemove = null;
            switch (typeOfQuestion)
            {
                case "colour":
                    animalsToRemove = animalArray.Select(m => (JToken)m).Where(n => n["colour"].ToString().ToLower() == value.ToLower()).ToArray();
                    break;
                case "sound":
                    animalsToRemove = animalArray.Select(m => (JToken)m).Where(n => n["sound"].ToString().ToLower() == value.ToLower()).ToArray();
                    break;
                case "feature":
                    animalsToRemove = animalArray.Select(m => (JToken)m).Where(n => n["feature"].ToString().ToLower() == value.ToLower()).ToArray();
                    break;
            }
            foreach (var animal in animalsToRemove)
            {
                animalArray.Remove(animal);
            }
        }

        /// <summary>
        /// This method removes any animal that lacks a certain attribute value
        /// </summary>
        /// <param name="typeOfQuestion"></param>
        /// <param name="value"></param>
        /// <param name="animalArray"></param>
        private static void RemoveNonMatchingAnimals(String typeOfQuestion, String value, JArray animalArray)
        {
            IList<JToken> animalsToRemove = null;
            switch (typeOfQuestion)
            {
                case "colour":
                    animalsToRemove = animalArray.Select(m => (JToken)m).Where(n => n["colour"].ToString().ToLower() != value.ToLower()).ToArray();
                    break;
                case "sound":
                    animalsToRemove = animalArray.Select(m => (JToken)m).Where(n => n["sound"].ToString().ToLower() != value.ToLower()).ToArray();
                    break;
                case "feature":
                    animalsToRemove = animalArray.Select(m => (JToken)m).Where(n => n["feature"].ToString().ToLower() != value.ToLower()).ToArray();
                    break;
            }
            foreach (var animal in animalsToRemove)
            {
                animalArray.Remove(animal);
            }        
        }

        /// <summary>
        /// This method checks if we can now guess which animal
        /// </summary>
        /// <param name="animalArray"></param>
        private static bool GuessTheAnimal(JArray animalArray)
        {
            bool finish = false;
            if (animalArray.Count == 0)
            {
                Console.WriteLine("We could not guess your animal!");
                finish = true;
            } else if (animalArray.Count == 1)
            {
                Console.WriteLine("The animal is: " + animalArray[0]["species"] + "!!!");
                finish = true;
            } else
            {
                finish = false;
            }
            return finish;
        }

        /// <summary>
        /// This method asks questions about the Animal
        /// </summary>
        /// <param name="typeOfQuestion"></param>
        private static bool AskQuestion(String typeOfQuestion, JToken animalObject)
        {
            bool match = false;
            string readChar = "";
            string readCharLower = "";
            do
            {               
                switch (typeOfQuestion)
                {
                    case "colour":
                        Console.Write("Is your chosen animal ");
                        Console.Write(animalObject["colour"]);
                        break;
                    case "sound":
                        Console.Write("Does your chosen animal make the following sound: ");
                        Console.Write(animalObject["sound"]);
                        break;
                    case "feature":
                        Console.Write("Does your chosen animal have the following feature: ");
                        Console.Write(animalObject["feature"]);
                        break;
                }                
                Console.WriteLine("? Yes (y) or No (n)");
               
                readChar = Console.ReadLine();
                readCharLower = readChar?.ToLower();
                if ((readCharLower == "y") || (readCharLower == "n"))
                    break;
            } while (true);

            if (readCharLower == "y")
            {
                match = true;
            }
            return match;
        }

        /// <summary>
        /// This method gets data for the Guess the Animal application      
        /// </summary>
        private static JArray GetApplicationData()
        {          
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directory = System.IO.Path.GetDirectoryName(path);

            JObject animalJSON = JObject.Parse(File.ReadAllText(directory + "/App_Data/AnimalData.json"));          
            JArray animalArray = animalJSON["animals"].Value<JArray>();
            return animalArray;
        }

        /// <summary>
        /// This method updates data for the Guess the Animal application      
        /// </summary>
        private static void UpdateApplicationData(JArray animalArray)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directory = System.IO.Path.GetDirectoryName(path);

            JObject animalJSON = JObject.Parse(File.ReadAllText(directory + "/App_Data/AnimalData.json"));
            animalJSON["animals"] = animalArray;

            File.WriteAllText(directory + "/App_Data/AnimalData.json", animalJSON.ToString());
        }
    }
}
