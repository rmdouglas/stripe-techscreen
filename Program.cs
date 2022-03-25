// // Step 1)
// //
// // Imagine an Airbnb-like vacation rental service, where users in different cities can exchange 
// // their apartment with another user for a week. Each user compiles a wishlist of the apartments 
// // they like. These wishlists are ordered, so the top apartment on a wishlist is that user's first 
// // choice for where they would like to spend a vacation. You will be asked to write part of the code 
// // that will help an algorithm find pairs of users who would like to swap with each other.
// //
// // Given a set of users, each with an *ordered* wishlist of other users' apartments:
// //
// // a's wishlist: c d
// // b's wishlist: d a c
// // c's wishlist: a b
// // d's wishlist: c a b
// //
// // The first user in each wishlist is the user's first-choice for whose apartment they would like 
// // to swap into.
// // Write a function called HasMutualFirstChoice() which takes a username and returns true if 
// // that user and another user are each other's first choice, and otherwise returns false.
// //
// // HasMutualFirstChoice('a') // true (a and c)
// // HasMutualFirstChoice('b') // false (b's first choice does not *mutually* consider b as their first choice)
// //
// // Then expand the base case beyond just "first" choices, to include all "mutually ranked choices". 
// // Write another function which takes a username and an option called "rank" to indicate the wishlist 
// // rank to query on. If given a rank of 0, you should check for a first choice pair, as before. If given 1, 
// // you should check for a pair of users who are each others' second-choice. Call your new function 
// // HasMutualPairForRank() and when done, refactor HasMutualFirstChoice() to depend on your new function.
// //
// // HasMutualPairForRank('a', 0) // true (a and c)
// // HasMutualPairForRank('a', 1) // true (a and d are mutually each others' second-choice)

// // Step 2)
// //
// // Every wishlist entry in the network is either "mutually ranked" or "not mutually ranked" depending on 
// // the rank the other user gives that user's apartment in return.
// //
// // The most common operation in the network is incrementing the rank of a single wishlist entry on a single 
// // user. This swaps the entry with the entry above it in that user's list. Imagine that, when this occurs, 
// // the system must recompute the "mutually-ranked-ness" of any pairings that may have changed.
// //
// // Write a function that takes a username and a rank representing the entry whose rank is being bumped up.
// // Return an array of the users whose pairings with the given user *would* gain or lose mutually-ranked 
// // status as a result of the change, if it were to take place. Call your function ChangedPairings()
// //
// // a's wishlist: c d
// // b's wishlist: d a c
// // c's wishlist: a b
// // d's wishlist: c a b
// //
// // // if d's second choice becomes their first choice, a and d will no longer be a mutually ranked pair
// // ChangedPairings('d', 1) // returns ['a']
// //
// // // if b's third choice becomes their second choice, c and b will become a mutually ranked pair (mutual 
// // second-choices)
// // ChangedPairings('b', 2) // returns ['c']
// //
// // // if b's second choice becomes their first choice, no mutually-ranked pairings are affected
// // ChangedPairings('b', 1) // returns []
// //


// // Step 3)
// //
// // A user's last choice is the last entry on their wishlist. Their second-to-last choice is second to last on 
// // their wishlist. This can be continued to define third-to-last choice, and so on, always counting from the 
// // end of the user's list of apartments.
// //
// // A mutually-ranked-anti-pairing is one where both parties rank each other's apartments identically near to 
// // (or far from) the *end* of each of their wishlists.
// //
// // Implement ChangedAntiPairings(username, rank) to return an array of the users whose pairings with the 
// // given user either gain or lose mutually-ranked-anti-pairing status as a result of the change.   Note that, 
// // as before, the username and rank passed in identify the entry whose rank is being bumped up, so (a, 1) 
// // would refer to a's second-choice.
// //
// // a's wishlist: c d
// // b's wishlist: d a c
// // c's wishlist: a b
// // d's wishlist: c a b
// //
// // // if b's third choice becomes their second choice, b and c will no longer be a mutually-ranked anti-pairing
// // ChangedAntiPairings('b', 2) // returns ['c']
// //
// // // if a's second choice becomes their first choice, a and c will be no longer be a mutually ranked anti-pairing
// // // in addition, a and d will become a mutually ranked anti-pairing (the second-to-last choice of each other)
// // ChangedAntiPairings('a', 1) // returns ['c', 'd']

// using System;
// using System.Collections.Generic;
// using System.Diagnostics.Contracts;
// using System.Linq;


// static class Extensions
// {
//     public static T At<T>(this IList<T> self, int index, bool fromBack) {        
//         return self[fromBack ? self.Count() - (index + 1) : index];
//     }
// }
// class Solution
// {
//     // Step 1)
//     static void Main(string[] args)
//     {
//         var data = Tests.MakeData();
//         Tests.TestHasMutualFirstChoice(data);
//         Tests.TestHasMutualPairForRank(data);
//         Tests.TestChangedPairings(data);
//         Tests.TestChangedAntiPairings(data);
//     }
    
//     private IDictionary<string, string[]> data;
//     public Solution(IDictionary<string, string[]> data) 
//     { 
//         this.data = data; 
//     }
    
//     public bool HasMutualFirstChoice(string username) {
//         return HasMutualPairForRank(username, 0);
//     }

//     public bool HasMutualPairForRank(string username, int rank) {
//         return InternalHasMutualPair(username, rank, rank);
//     }
    
//     public bool InternalHasMutualPair(string username, int rank1, int rank2, bool fromBack = false) {
//         if(rank1 < 0) return false;
//         if(rank2 < 0) return false;
          
//         // If there's no wishlist for the username, return false
//         if(!data.ContainsKey(username)) return false;
        
//         // Get the users wishlist
//         var wishlist1 = data[username];
//         // If the users wishlist is null or empty, return false
//         if(wishlist1 == null || wishlist1.Count() <= rank1) return false;        
//         // Get the user from the head of the wishlist
//         var otherUser = wishlist1.At(rank1, fromBack);
//         // If the other user has no wishlist, return false
//         if(!data.ContainsKey(otherUser)) return false;

//         // Get the wishlist for the other user
//         var wishlist2 = data[otherUser];
//         // If th other users wishlist is null or empty, return false
//         if(wishlist2 == null || wishlist2.Count() <= rank2) return false;
//         // check if the other user has the given user as their first choice
//         return username == wishlist2.At(rank2, fromBack);
//     }

//     public string[] ChangedPairings(string username, int rank) {
//         var changed = new List<string>();
        
//         if(InternalHasMutualPair(username, rank, rank) != InternalHasMutualPair(username, rank, rank - 1))
//             changed.Add(data[username][rank]);
//         if(InternalHasMutualPair(username, rank - 1, rank - 1) != InternalHasMutualPair(username, rank - 1, rank))
//             changed.Add(data[username][rank-1]);
        
//         var array = changed.ToArray();
//         Array.Sort(array);
//         return array;   
//     }
    
//     public string[] ChangedAntiPairings(string username, int rank) {
//         var changed = new List<string>();
        
//         if(!data.ContainsKey(username)) return new string[] {};
//         rank = data[username].Count() - (rank+1);
        
//         if(InternalHasMutualPair(username, rank, rank, true) != InternalHasMutualPair(username, rank, rank + 1, true))
//             changed.Add(data[username].At(rank, true));
//         if(InternalHasMutualPair(username, rank + 1, rank + 1, true) != InternalHasMutualPair(username, rank + 1, rank, true))
//             changed.Add(data[username].At(rank+1, true));

//         var array = changed.ToArray();
//         Array.Sort(array);
//         return array;   
//     }
// }

// class Tests {
//     public static IDictionary<string, string[]> MakeData() {
//         return new Dictionary<string, string[]>
//         {
//             ["a"] = new string[] { "c", "d" },
//             ["b"] = new string[] { "d", "a", "c" },
//             ["c"] = new string[] { "a", "b" },
//             ["d"] = new string[] { "c", "a", "b" },
//             ["e"] = new string[] {},
//             ["f"] = new string[] { "e", },
//             ["g"] = new string[] { "g", },
//         };
//     }

//     public static void AssertEqual<T>(T actual, T expected) where T : System.IComparable<T> {
//         Contract.Assert(expected.CompareTo(actual) == 0, 
//                 "Expected:\n  " + expected + 
//                 "\nActual:\n  " + actual + 
//                 "\n");
//         Console.WriteLine("PASSED");
//     }

//     public static void AssertEqual<T>(IEnumerable<T> actual, IEnumerable<T> expected) where T : System.IComparable<T> {
//         Contract.Assert(expected.SequenceEqual(actual), 
//                 "Expected:\n  " + $"[ {String.Join(", ", expected)} ]" + 
//                 "\nActual:\n  " + $"[ {String.Join(", ", actual)} ]" + 
//                 "\n");
//         Console.WriteLine("PASSED");
//     }
  
//     public static void TestHasMutualFirstChoice(IDictionary<string, string[]> data) {
//         AssertEqual(new Solution(data).HasMutualFirstChoice("a"), true);
//         AssertEqual(new Solution(data).HasMutualFirstChoice("b"), false);
//         AssertEqual(new Solution(data).HasMutualFirstChoice("z"), false);
//         AssertEqual(new Solution(data).HasMutualFirstChoice("e"), false);
//         AssertEqual(new Solution(data).HasMutualFirstChoice("f"), false);
//         AssertEqual(new Solution(data).HasMutualFirstChoice("g"), true);
//     }
  
//     public static void TestHasMutualPairForRank(IDictionary<string, string[]> data) {
//         AssertEqual(new Solution(data).HasMutualPairForRank("a", 0), true);
//         AssertEqual(new Solution(data).HasMutualPairForRank("a", 1), true);
//     }
    
//     public static void TestChangedPairings(IDictionary<string, string[]> data) {
//         // if d's second choice becomes their first choice, a and d 
//         // will no longer be a mutually ranked pair
//         AssertEqual(new Solution(data).ChangedPairings("d", 1), new string[]{"a"});

//         // if b's third choice becomes their second choice, c and b 
//         // will become a mutually ranked pair (mutual second-choices)
//         AssertEqual(new Solution(data).ChangedPairings("b", 2), new string[]{"c"});

//         // if b's second choice becomes their first choice, no 
//         // mutually-ranked pairings are affected
//         AssertEqual(new Solution(data).ChangedPairings("b", 1), new string[]{});
//     }
    
//     public static void TestChangedAntiPairings(IDictionary<string, string[]> data) {
//         // if b's third choice becomes their second choice, 
//         // b and c will no longer be a mutually-ranked anti-pairing
//         AssertEqual(new Solution(data).ChangedAntiPairings("b", 2), new string[]{"c"});

//         // if a's second choice becomes their first choice, 
//         // a and c will be no longer be a mutually ranked anti-pairing
//         // in addition, a and d will become a mutually ranked anti-pairing 
//         // (the second-to-last choice of each other)
//         AssertEqual(new Solution(data).ChangedAntiPairings("a", 1), new string[]{"c", "d"});
//     }
// }