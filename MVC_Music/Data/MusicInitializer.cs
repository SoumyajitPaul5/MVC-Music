using Microsoft.EntityFrameworkCore;
using MVC_Music.Models;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

namespace MVC_Music.Data
{
    public static class MusicInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            MusicContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<MusicContext>();

            try
            {

                context.Database.Migrate();

                Random random = new Random();

                string[] genres = new string[] { "Classical", "Rock", "Pop", "Jazz", "Country", "Ambient", "Techno" };
                string[] baconNotes = new string[] { "Bacon ipsum dolor amet meatball corned beef kevin, alcatra kielbasa biltong drumstick strip steak spare ribs swine. Pastrami shank swine leberkas bresaola, prosciutto frankfurter porchetta ham hock short ribs short loin andouille alcatra. Andouille shank meatball pig venison shankle ground round sausage kielbasa. Chicken pig meatloaf fatback leberkas venison tri-tip burgdoggen tail chuck sausage kevin shank biltong brisket.", "Sirloin shank t-bone capicola strip steak salami, hamburger kielbasa burgdoggen jerky swine andouille rump picanha. Sirloin porchetta ribeye fatback, meatball leberkas swine pancetta beef shoulder pastrami capicola salami chicken. Bacon cow corned beef pastrami venison biltong frankfurter short ribs chicken beef. Burgdoggen shank pig, ground round brisket tail beef ribs turkey spare ribs tenderloin shankle ham rump. Doner alcatra pork chop leberkas spare ribs hamburger t-bone. Boudin filet mignon bacon andouille, shankle pork t-bone landjaeger. Rump pork loin bresaola prosciutto pancetta venison, cow flank sirloin sausage.", "Porchetta pork belly swine filet mignon jowl turducken salami boudin pastrami jerky spare ribs short ribs sausage andouille. Turducken flank ribeye boudin corned beef burgdoggen. Prosciutto pancetta sirloin rump shankle ball tip filet mignon corned beef frankfurter biltong drumstick chicken swine bacon shank. Buffalo kevin andouille porchetta short ribs cow, ham hock pork belly drumstick pastrami capicola picanha venison.", "Picanha andouille salami, porchetta beef ribs t-bone drumstick. Frankfurter tail landjaeger, shank kevin pig drumstick beef bresaola cow. Corned beef pork belly tri-tip, ham drumstick hamburger swine spare ribs short loin cupim flank tongue beef filet mignon cow. Ham hock chicken turducken doner brisket. Strip steak cow beef, kielbasa leberkas swine tongue bacon burgdoggen beef ribs pork chop tenderloin.", "Kielbasa porchetta shoulder boudin, pork strip steak brisket prosciutto t-bone tail. Doner pork loin pork ribeye, drumstick brisket biltong boudin burgdoggen t-bone frankfurter. Flank burgdoggen doner, boudin porchetta andouille landjaeger ham hock capicola pork chop bacon. Landjaeger turducken ribeye leberkas pork loin corned beef. Corned beef turducken landjaeger pig bresaola t-bone bacon andouille meatball beef ribs doner. T-bone fatback cupim chuck beef ribs shank tail strip steak bacon." };
                string[] firstNames = new string[] { "Lyric", "Antoinette", "Kendal", "Vivian", "Ruth", "Jamison", "Emilia", "Natalee", "Yadiel", "Jakayla", "Lukas" };//, "Moses", "Kyler", "Karla", "Chanel", "Tyler", "Camilla", "Quintin", "Braden", "Clarence" };
                string[] lastNames = new string[] { "Watts", "Randall", "Arias", "Weber", "Stone", "Carlson", "Robles", "Frederick" };//, "Parker", "Morris", "Soto", "Bruce", "Orozco", "Boyer", "Burns", "Cobb", "Blankenship", "Houston", "Estes", "Atkins", "Miranda", "Zuniga", "Ward", "Mayo", "Costa", "Reeves", "Anthony", "Cook", "Krueger", "Crane", "Little", "Henderson", "Bishop" };

                // Genre
                if (!context.Genres.Any())
                {
                    // looping through the array of Genre names
                    foreach (string g in genres)
                    {
                        Genre genre = new()
                        {
                            Name = g
                        };
                        context.Genres.Add(genre);
                    }
                    context.SaveChanges();
                }


                int[] genreIDs = context.Genres.Select(g => g.ID).ToArray();
                int genreIDCount = genreIDs.Length;

                // Album
                if (!context.Albums.Any())
                {
                    context.Albums.AddRange(
                     new Album
                     {
                         Name = "Rocket Food",
                         YearProduced = "2000",
                         Price = 19.99d,
                         GenreID = genreIDs[random.Next(genreIDs.Count())]
                     },
                     new Album
                     {
                         Name = "Songs of the Sea",
                         YearProduced = "1999",
                         Price = 9.99d,
                         GenreID = genreIDs[random.Next(genreIDs.Count())]
                     },
                     new Album
                     {
                         Name = "The Horse",
                         YearProduced = "1929",
                         Price = 99.99d,
                         GenreID = genreIDs[random.Next(genreIDs.Count())]
                     },
                     new Album
                     {
                         Name = "Music From Away",
                         YearProduced = "1999",
                         Price = 9.99d,
                         GenreID = genreIDs[random.Next(genreIDs.Count())]
                     },
                     new Album
                     {
                         Name = "Life",
                         YearProduced = "1988",
                         Price = 19.99d,
                         GenreID = genreIDs[random.Next(genreIDs.Count())]
                     },
                     new Album
                     {
                         Name = "Small Minds, Big Hearts",
                         YearProduced = "1967",
                         Price = 12.99d,
                         GenreID = genreIDs[random.Next(genreIDs.Count())]
                     },
                     new Album
                     {
                         Name = "The Cow",
                         YearProduced = "2010",
                         Price = 21.99d,
                         GenreID = genreIDs[random.Next(genreIDs.Count())]
                     },
                     new Album
                     {
                         Name = "Freedom",
                         YearProduced = "2012",
                         Price = 29.99d,
                         GenreID = genreIDs[random.Next(genreIDs.Count())]
                     });
                    context.SaveChanges();
                }

                // Creating a collection of the primary keys of the Albums
                int[] albumIDs = context.Albums.Select(a => a.ID).ToArray();
                int albumIDCount = albumIDs.Length;

                // Song
                if (!context.Songs.Any())
                {

                    DateTime startDate = DateTime.Today;


                    foreach (string f in firstNames)
                    {
                        foreach (string g in lastNames)
                        {
                            Song s = new()
                            {
                                Title = f + " " + g,
                                DateRecorded = startDate.AddDays(-random.Next(30, 1000)),
                                GenreID = genreIDs[random.Next(genreIDCount)],
                                AlbumID = albumIDs[random.Next(albumIDCount)]
                            };
                            try
                            {
                                context.Songs.Add(s);
                                context.SaveChanges();
                            }
                            catch (Exception)
                            {
                                context.Songs.Remove(s);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
