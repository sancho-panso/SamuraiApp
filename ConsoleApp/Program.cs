using System;
using Microsoft.EntityFrameworkCore.Storage;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    internal class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        private static void Main(string[] args)
        {
            _context.Database.EnsureCreated();
            //GetSamurais("Before Add:");
            //AddSamurai();
            //InsertMultipleSamurais();
            //GetSamurais("After Add");
            //QueryFilters();
            //RetrieveAndUpdate();
            //InsertBattle();
            //QueryAndUpdateBatttle_Disconected();
            //InsertNewSamuraiWithManyQuote();
            //AddQuoteToExistingSamurai();
            //AddQuoteToExistingSamuraiNotTracked(3);
            //ExplicitLoadQuotes();
            // FilteringWithelatedData();
            //ModifyRelatedDataWhenTracked();
            //QueryUsingRawSQL();
            Console.WriteLine("Press any key..");
            Console.ReadKey();

        }

        private static void InsertMultipleSamurais()
        {
            var samuraisList = new List<Samurai>
            {
                new Samurai { Name = "Tasha"},
                new Samurai { Name = "Oskar"},
                new Samurai { Name = "Persi"}
            };
            _context.Samurais.AddRange(samuraisList);
            _context.SaveChanges();
        }
        private static void InsertVariousTypes()
        {
            var samurai = new Samurai { Name = "Kikuchio" };
            var clan = new Clan { ClanName = "imperial Clan" };
            _context.AddRange(samurai, clan);
            _context.SaveChanges();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Julie" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }

        private static void QueryFilters()
        {
            var name = "Sampson";
            var samurais = _context.Samurais.Where(s => s.Name == name ).ToList();
            var samurais2 = _context.Samurais.FirstOrDefault(samurais => samurais.Name == name);
            var filter = "J%";
            var samurai = _context.Samurais.Where(s => EF.Functions.Like(s.Name,filter)).ToList();
            var last = _context.Samurais.OrderBy(s => s.Id).LastOrDefault(samurai => samurai.Name == name);
        }
        private static void RetrieveAndUpdate()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }  
        private static void MultiplyDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.Samurais.Add(new Samurai { Name = "Nichiko" });
            _context.SaveChanges();
        } 
        private static void RetrieveAndUpdateMulti()
        {
            var samurais = _context.Samurais.Skip(1).Take(3).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        } 
        private static void RetrieveAndDelete()
        {
            var samurai = _context.Samurais.Find(5);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void InsertBattle()
        {
            _context.Battles.Add(new Battle
            {
                Name = "Battle of Okehazama",
                StartDate = new DateTime(1560, 05, 01),
                EndTime = new DateTime(1560, 06, 15)
            });
            _context.SaveChanges();
        }
        private static void QueryAndUpdateBatttle_Disconected()
        {
            var battle = _context.Battles.AsNoTracking().FirstOrDefault();
            battle.EndTime = new DateTime(1560,06,30);
            using (var newConntextInstance = new SamuraiContext())
            {
                newConntextInstance.Battles.Update(battle);
                newConntextInstance.SaveChanges();
            }
        }

        private static void InsertNewSamuraiWithQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "I've come to save you"}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }   
        
        private static void InsertNewSamuraiWithManyQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kyozu",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "Watch out for my sharp sword"},
                    new Quote { Text = "I told you to watch out for the sharp sword! Oh well!"}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }  
        private static void AddQuoteToExistingSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(
                    new Quote { Text = "I  one more time bet you are happy that I have saved you"}
                );
            _context.SaveChanges();
        }  
        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(
                    new Quote { Text = "Now that I saved you will you feed me dinner?"}
                );
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Update(samurai);
                //newContext.Samurais.Attach(samurai);
                newContext.SaveChanges();
            }
        }     
        private static void AddQuoteToExistingSamuraiNotTracked_ForegnKey(int samuraiId)
        {
            var quote = new Quote{ Text = "Now that I saved you will you feed me dinner?"};
            using (var newContext = new SamuraiContext())
            {
                newContext.Quotes.Add(quote);

                newContext.SaveChanges();
            }
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            var samuraiWithQuotes = _context.Samurais.Where(s => s.Name.Contains("Julie"))
                                                      .Include(s => s.Quotes).FirstOrDefault();
        }

        private static void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Name, s.Id,
                                                   HappyQuotes=s.Quotes.Where(q=>q.Text.Contains("happy"))})
                                                  .ToList();
            var samuraiWithHappyQuotes = _context.Samurais.Select(s => new
            {
                Samurai = s,
                HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
            }).ToList();
            var firstsamurai = samuraiWithHappyQuotes[0].Samurai.Name += "Happiest";
        }

        private static void ExplicitLoadQuotes()
        {
            var samurai = _context.Samurais.FirstOrDefault(samurai => samurai.Name.Contains("Julie"));
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            _context.Entry(samurai).Reference(s => s.Horse).Load();

        }
        private static void FilteringWithelatedData()
        {
            var samurai = _context.Samurais
                .Where(s => s.Quotes.Any(q => q.Text.Contains("sword")))
                .ToList();

        }
        private static void ModifyRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s=>s.Quotes).FirstOrDefault(s => s.Id == 7);
            samurai.Quotes[0].Text = "Did You hear that?";
            _context.Quotes.Remove(samurai.Quotes[1]);
            _context.SaveChanges();
              

        }
        
        private static void ModifyRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s=>s.Quotes).FirstOrDefault(s => s.Id == 7);
            var quote = samurai.Quotes[0];
            quote.Text = "Did You hear that again?";
            using (var newContext = new SamuraiContext())
            {
                //newContext.Quotes.Update(quote); bad example
                newContext.Entry(quote).State = EntityState.Modified;
                    _context.SaveChanges();
            }
        }

        private static void JoinBattleAndSamurai()
        {
            var sbJoin = new SamuraiBattle { SamuraiId = 1, BatlleId = 1 };
            _context.Add(sbJoin);
            _context.SaveChanges();
        }

        private static void EnlistSamuraiIntoBattle()
        {
            var battle = _context.Battles.Find(1);
            battle.SamuraiBattles
                .Add(new SamuraiBattle { SamuraiId = 21 });
            _context.SaveChanges();
        }

        private static void RemoveJoin()
        {
            var join = new SamuraiBattle { BatlleId = 1, SamuraiId = 2 };
            _context.Remove(join);
            _context.SaveChanges();
        }


        private static void GetSamuraiWithBattles()
        {
            var samuraiWithBattle = _context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle)
                .FirstOrDefault(Samurai => Samurai.Id == 2);

            var samuraiWithBattlesCleaner = _context.Samurais.Where(s => s.Id == 2)
                .Select(s => new
                {
                    Samurai = s,
                    Battles = s.SamuraiBattles.Select(sb => sb.Battle)
                }).FirstOrDefault();
        }

        private static void AddNewSamuraiWithHorse()
        {
            var samurai = new Samurai { Name = "JIna Ujichika" };
            samurai.Horse = new Horse { Name = "Silver" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddNiwHorseToSamuraiUsingId()
        {
            var horse = new Horse { Name = "Scout", SamuraiId = 2 };
            _context.Add(horse);
            _context.SaveChanges();
        } 
        
        private static void AddNiwHorseToSamuraiObject()
        {
            var samurai = _context.Samurais.Find(2);
            samurai.Horse =new Horse { Name = "Scoutty"};
            _context.SaveChanges();
        }

        private static void AddNewHorseToDisconectedSamuraiObject()
        {
            var samurai = _context.Samurais.AsNoTracking().FirstOrDefault(samurai => samurai.Id == 2);
            samurai.Horse = new Horse { Name = "Scoutty" };
            using (var newContext = new SamuraiContext())
            {
                newContext.Attach(samurai);
                newContext.SaveChanges();

            }

        }

        private static void GetSamuraiWithHorse()
        {
            var samurai = _context.Samurais.Include(s => s.Horse).ToList();
        }

        private static void GetClanWithSamurais()
        {
            var clan = _context.Clans.Find(3);
            var samuraisForClan = _context.Samurais.Where(s => s.Clan.Id == 3).ToList();
        }

        private static void QueryUsingRawSQL()
        {
            var samurais = _context.Samurais.FromSqlRaw("Select * from Samurais").ToList();
        }

        private static void QueryUsingRawSqlWithInterpolation()
        {
            string name = "Kikuchyo";
            var samurais = _context.Samurais
                .FromSqlInterpolated($"Select * from Samurais Where Name = {name}")
                .ToList();
        }

    }
}
