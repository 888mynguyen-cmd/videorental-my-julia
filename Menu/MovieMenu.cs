using MySqlConnector;

public class MovieMenu
{
    public static void ShowMovieMenu(MySqlConnection connection)
    {
        while (true)
                {
                    Console.WriteLine("---------Filmer--------");
                    Console.WriteLine("Välj nedan?");

                    Console.WriteLine("1. Visa alla Filmer");
                    Console.WriteLine("2. Lägg till film");
                    Console.WriteLine("3. Ändra film");
                    Console.WriteLine("4. Ta bort");
                    Console.WriteLine("5. Hantera fysiska exempel");
                    Console.WriteLine("6. Gå tillbaka till huvudmenyn");

                    Console.WriteLine("Välj alternativ: ");
                    int FilmChoice = Convert.ToInt32(Console.ReadLine());


                    if (FilmChoice == 1)
                    {
                        // Visa alla filmer
                        List<Movie> movies = MovieRepository.ShowMovie(connection);

                        foreach (Movie movie in movies)
                        {
                            Console.WriteLine(
                                $"{movie.Id} | {movie.Title} | {movie.Description} | PrisId: {movie.RentalPriceId}");
                        }
                    }

                    else if (FilmChoice == 2)
                    {
                        // Lägg till film
                        Console.WriteLine("Lägg till en titel på film ");
                        string Title = Console.ReadLine() ?? "";

                        Console.WriteLine("Lägg till en beskrivning på film ");
                        string Description = Console.ReadLine() ?? "";

                        // 1 = Ny film, 2 = Gammal film
                        Console.WriteLine("Vilket id på priset till filmen?");
                        int RentalPriceId = Convert.ToInt32(Console.ReadLine() ?? "");

                        MovieRepository.AddMovie(
                            connection,
                            Title,
                            Description,
                            RentalPriceId);
                    }

                    else if (FilmChoice == 3)
                    {
                        // Ändra film
                        Console.WriteLine("Vilken film vill du ändra?");
                        int changeId = Convert.ToInt32(Console.ReadLine() ?? "");

                        Console.WriteLine("Vad vill du ändra filmens titel till?");
                        string changeTitle = Console.ReadLine() ?? "";

                        Console.WriteLine("Vad vill du ändra filmens beskrivning till?");
                        string changeDescription = Console.ReadLine() ?? "";

                        Console.WriteLine("Vad vill du ändra filmens Pris till?");
                        int changeRentalPriceId = Convert.ToInt32(Console.ReadLine() ?? "");

                        MovieRepository.ChangeMovie(
                            connection,
                            changeId,
                            changeTitle,
                            changeDescription,
                            changeRentalPriceId);
            }


            else if (FilmChoice == 4)
            {
                // Ta bort film
                Console.WriteLine("Vilken film vill du ta bort?");
                int removeMovieId = Convert.ToInt32(Console.ReadLine() ?? "");

                MovieRepository.RemoveMovie(connection, removeMovieId);
                Console.WriteLine("Filmen har tagits bort!");
            }

            else if (FilmChoice == 5)
                {
                    while(true)
                    {
                        Console.WriteLine("---------Fysiska Exemplar--------");
                        Console.WriteLine("Välj nedan?");

                        Console.WriteLine("1. Visa alla exemplar");
                        Console.WriteLine("2. Lägg till exemplar");
                        Console.WriteLine("3. Ändra exemplarets status");
                        Console.WriteLine("4. Tillbaka till Filmer");

                        Console.WriteLine("Välj alternativ: ");
                        int PhysicalChoice = Convert.ToInt32(Console.ReadLine());

                        if (PhysicalChoice == 1)
                        {
                            List<PhysicalCopy> physicalCopies = PhysicalCopyRepository.ShowPhysicalCopy(connection);

                            foreach (PhysicalCopy physicalCopy in physicalCopies)
                            {
                                Console.WriteLine(
                                    $"{physicalCopy.Id} | {physicalCopy.BarCode} | {physicalCopy.MovieId} | {physicalCopy.MovieTitle} | {physicalCopy.FilmStatusId} | {physicalCopy.StatusName}");
                            }
                        }

                        else if (PhysicalChoice == 2)
                        {
                            // Lägg till fysiskt exemplay
                            Console.WriteLine("Ange Id för filmen som exemplaret tillhör:");
                            int movieId = Convert.ToInt32(Console.ReadLine() ?? "");

                            Console.WriteLine("Ange streckkoden för det fysiska exemplaret:");
                            string barCode = Console.ReadLine() ?? "";

                            Console.WriteLine("Ange Id för statusen på en film som finns!");
                            int filmStatusId = Convert.ToInt32(Console.ReadLine() ?? "");


                            PhysicalCopyRepository.AddPhysicalCopy(
                                connection,
                                barCode,
                                movieId,
                                filmStatusId);

                        }

                        else if (PhysicalChoice == 3)
                        {
                            // Ändra fysiska exemplaren
                             Console.WriteLine("Ange Id för exemplaret du vill ändra status på:");
                            if (!int.TryParse(Console.ReadLine(), out int physicalId))
                            {
                                Console.WriteLine("Ogiltigt Id!");
                                continue;
                            }

                            Console.WriteLine("Ange Id för den nya statusen:");
                            if (!int.TryParse(Console.ReadLine(), out int filmStatus))
                            {
                                Console.WriteLine("Ogiltigt Id!");
                                continue;
                            }

                            PhysicalCopyRepository.ChangePhysicalCopy(connection, physicalId, filmStatus);
                        }

                    else if (PhysicalChoice == 4)
                    {
                        Console.WriteLine("Tillbaka till Filmer!");
                        break;
                    }
                }
            }

            else if (FilmChoice == 6) 
            {
                Console.WriteLine("Tillbaka till Huvudmenyn!");
                break;
            }
        }
    }    
}