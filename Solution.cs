using System;
using System.Collections;
using System.Collections.Generic;

/* Program: Solution.cs
 * Author: Dipesh Nainani   dsn1945@rit.edu
 * 
 * This program implements the class LinkedHashTable 
 * which inherits the table interface. The LinkedHashTable class
 * implements all the methods in the table interface. It also takes
 * care of rehashing and it implements array of linkedlist to 
 * store the key value pair on the index obtained by default hashcode
 * method of C#
 * 
 * This program also implements TestTable class which contains
 * different test cases.
 * 
 * 
 */

namespace RIT_CS
{
	/// <summary>
	/// An exception used to indicate a problem with how
	/// a HashTable instance is being accessed
	/// </summary>
	public class NonExistentKey<Key> : Exception
	{
		/// <summary>
		/// The key that caused this exception to be raised
		/// </summary>
		public Key BadKey { get; private set; }

		/// <summary>
		/// Create a new instance and save the key that
		/// caused the problem.
		/// </summary>
		/// <param name="k">
		/// The key that was not found in the hash table
		/// </param>
		public NonExistentKey(Key k) :
			base("Non existent key in HashTable: " + k)
		{
			BadKey = k;
		}

	}

	/// <summary>
	/// Provided by Professor Brown
	/// An associative (key-value) data structure.
	/// A given key may not appear more than once in the table,
	/// but multiple keys may have the same value associated with them.
	/// Tables are assumed to be of limited size are expected to automatically
	/// expand if too many entries are put in them.
	/// </summary>
	/// <param name="Key">the types of the table's keys (uses Equals())</param>
	/// <param name="Value">the types of the table's values</param>
	interface Table<Key, Value> : IEnumerable<Key>
	{
		/// <summary>
		/// Add a new entry in the hash table. If an entry with the
		/// given key already exists, it is replaced without error.
		/// put() always succeeds.
		/// (Details left to implementing classes.)
		/// </summary>
		/// <param name="k">the key for the new or existing entry</param>
		/// <param name="v">the (new) value for the key</param>
		void Put(Key k, Value v);

		/// <summary>
		/// Does an entry with the given key exist?
		/// </summary>
		/// <param name="k">the key being sought</param>
		/// <returns>true iff the key exists in the table</returns>
		bool Contains(Key k);

		/// <summary>
		/// Fetch the value associated with the given key.
		/// </summary>
		/// <param name="k">The key to be looked up in the table</param>
		/// <returns>the value associated with the given key</returns>
		/// <exception cref="NonExistentKey">if Contains(key) is false</exception>
		Value Get(Key k);


	}


	/// <summary>
	/// Provided by Professor Brown
	/// This class creates a Table Factory which returns an object
	/// of LinkedHashTable class
	/// </summary>
	class TableFactory<K, V>
	{
		/// <summary>
		/// Create a Table.
		/// Returns the object of the LinkedHashTable class
		/// </summary>
		/// <param name="K">the key type</param>
		/// <param name="V">the value type</param>
		/// <param name="capacity">The initial maximum size of the table</param>
		/// <param name="loadThreshold">
		/// The fraction of the table's capacity that when
		/// reached will cause a rebuild of the table to a 50% larger size
		/// </param>
		/// <returns>A new instance of Table</returns>
		public static Table<K, V> Make<K, V>(int capacity = 100, double loadThreshold = 0.75)
		{
			return new LinkedHashTable<K, V>(capacity, loadThreshold);
		}

	}


	class HashNodes<K, V>
	{
		/// <summary>
		/// Used by LinkedList created in class LinkedHashTable to
		/// create nodes of Key Value pair.
		/// </summary>
		/// <param name="key">the key for the new or existing entry</param>
		/// <param name="val">the (new) value for the key</param>
		/// 
		public K key;
		public V val;

	}


	class LinkedHashTable<K, V> : Table<K, V>
	{
		/// <summary>
		/// This class implements the methods of the Table interface. It
		/// also contains a constructor which intializes the values obtained
		/// from the TableFactory.
		/// </summary>
		/// <param name="K">the key for the new or existing entry</param>
		/// <param name="V">the (new) value for the key</param>
		/// <param name="capacity">The initial maximum size of the table</param>
		/// <param name="loadThreshold">
		/// The fraction of the table's capacity that when
		/// reached will cause a rebuild of the table to a 50% larger size
		/// </param>
		/// <param name = "keyArray"> It creates an array of keys in the LinkedList</param>
		/// <param name = "listType"> 
		/// It contains the objects that is to be stored in the LinkedList 
		/// </param>
		/// <param name = "number">The number of nodes in the LinkedList</param>

		int capacity;
		object[] listType;
		K[] keyArray;
		int keycount = 0;
		int number = 0;
		double loadThreshold;


		/// <summary>
		/// This is the constructor of the class LinkedHashTable which initializes the
		/// values and adds LinkedList in all the indexes of the array.
		/// </summary>
		/// <param name="capacity">The initial maximum size of the table</param>
		/// <param name="loadThreshold">
		/// The fraction of the table's capacity that when
		/// reached will cause a rebuild of the table to a 50% larger size
		/// </param>

		public LinkedHashTable(int capacity, double loadThreshold)
		{
			this.capacity = capacity;
			this.loadThreshold = loadThreshold;
			listType = new Object[capacity];
			keyArray = new K[capacity];
			for (int i = 0; i < capacity; i++)
			{
				listType[i] = new LinkedList<HashNodes<K, V>>();
			}

		}

		/// <summary>
		/// Checks whether an entry exist in the LinkedList or not
		/// </summary>
		/// <param name="k">the key being sought</param>
		/// <returns>true iff the key exists in the table</returns>

		public bool Contains(K k)
		{
			// gets the hash code of the key
			int code = k.GetHashCode();
			int i = code % capacity;
			if (i < 0)
				i = i + capacity;
			// get the list present at the index of the array
			LinkedList<HashNodes<K, V>> newTemp = (LinkedList<HashNodes<K, V>>)listType[i];
			// checks whether the key is present in the linked list or not
			if (newTemp.Contains(new HashNodes<K, V>() { key = k }))
				return true;

			return false;
		}

		/// <summary>
		/// Fetch the value associated with the given key. It thows an
		/// exception when the key is not present in the LinkedList.
		/// </summary>
		/// <param name="k">The key to be looked up in the table</param>
		/// <returns>the value associated with the given key</returns>
		/// <exception cref="NonExistentKey">if Contains(key) is false</exception>

		public V Get(K k)
		{
			// checks whether the given key is null or not
			if (k != null)
			{
				// gets the hash code of the key 
				int code = k.GetHashCode();

				int i = code % capacity;
				if (i < 0)
					i = i + capacity;
				// gets the list present at that index
				LinkedList<HashNodes<K, V>> ll = (LinkedList<HashNodes<K, V>>)listType[i];
				var head = ll.First;
				// gets the value by comparing the key in linked list present at that index
				while (head != null)
				{
					if (head.Value.key.Equals(k))
						return head.Value.val;
					head = head.Next;
				}
			}
			throw new NonExistentKey<K>(k);
		}


		/// <summary>
		/// This iterates through the LinkedList stored in the object of 
		/// the array and returns keys in the LinkedList.
		/// </summary>
		/// <returns>the key in the LinkedList</returns>

		public IEnumerator<K> GetEnumerator()
		{
			// traverses the keys and returns them 
			foreach (K k in keyArray)
				if(k!=null)
					yield return k;
			
		}

		/// <summary>
		/// Add a new entry in the hash table. If an entry with the
		/// given key already exists, it is replaced without error.
		/// This function also rehashes when it gets full to a 
		/// size of 50% larger.
		/// </summary>
		/// <param name="k">the key for the new or existing entry</param>
		/// <param name="v">the (new) value for the key</param>

		public void Put(K k, V v)
		{
			// checks whether the table needs to be rehashed
			if (loadThreshold <= number / (float)capacity)
			{
				Console.WriteLine("           ");
				Console.WriteLine(" Table is Full!! Now Hashing");
				Console.WriteLine("           ");
				int newCapacity = (int)1.5 * capacity;
				Object[] newListType = new Object[newCapacity];
				for (int i = 0; i < newCapacity; i++)
				{
					newListType[i] = new LinkedList<HashNodes<K, V>>();
				}
				for (int i = 0; i < listType.Length; i++)
				{
					LinkedList<HashNodes<K, V>> llist = (LinkedList<HashNodes<K, V>>)listType[i];
					foreach (HashNodes<K, V> hashnode in llist)
					{
						int hashcode = hashnode.GetHashCode();
						int index = hashcode % newCapacity;
						if (index < 0)
							index = index + newCapacity;
						LinkedList<HashNodes<K, V>> ll = (LinkedList<HashNodes<K, V>>)newListType[index];
						ll.AddFirst(new HashNodes<K, V>() { key = k, val = v });

					}
				}
			}
			// gets the hash code for the key
			int code = k.GetHashCode();
			int j = code % capacity;
			if (j < 0)
				j = j + capacity;
			LinkedList<HashNodes<K, V>> linkedList = (LinkedList<HashNodes<K, V>>)listType[j];
			var head = linkedList.First;
			// traverses the LinkedList at the index of the array
			while (head != null)
			{
				if (head.Value.key.Equals(k))
				{
					head.Value.val = v;
					return;
				}
				head = head.Next;
			}

			// adds the item in the linkedlist after traversing it
			linkedList.AddFirst(new HashNodes<K, V>() { key = k, val = v });
			number++;
			keyArray[keycount] = k;
			keycount++;
		}

		/// <summary>
		/// Returns an object of GetEnumerator method
		/// </summary>
		/// <returns>object of the GetEnumerator method</returns>

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator<K>)GetEnumerator();
		}
	}

	/// <summary>
	/// This class implements test method which calls several test cases
	/// </summary>

	class TestTable
	{
		/// <summary>
		/// This method executes several test cases on the LinkedHashTable class.
		/// </summary>
		public static void test()
		{
			
			try
			{
			LinkedHashTable<String, int> ht = (LinkedHashTable<String, int>)TableFactory<String, int>.Make<String, int>(10, 0.5);
			ht.Put("dipesh", 2);
			ht.Put("trudy", 6);
			ht.Put("rob", 10);
			ht.Put("ron", 5);
			ht.Put("danny", 8);

				/// <summary>
				/// This test case adds integers in the table and test's for contains method.
				/// </summary>
				Console.WriteLine("           ");
				Console.WriteLine("Test Case 3");
				Console.WriteLine("           ");

				foreach (String first in ht)
				{
					Console.WriteLine(first + " -> " + ht.Get(first));
				}

				/// <summary>
				/// This test case checks whether same key is replaced with another value or not.
				/// </summary>
				ht.Put("how",3);
				ht.Put("how",4);
				ht.Put("are", 9);
				foreach (String first in ht)
				{
					Console.WriteLine(first + " -> " + ht.Get(first));
				}
				Console.WriteLine("=========================");
				Console.WriteLine("           ");
				Console.WriteLine("Test Case 4");
				Console.WriteLine("           ");

				Console.Write("how -> ");
				Console.WriteLine(ht.Get("how"));
				Console.Write("are -> ");
				Console.WriteLine(ht.Get("are"));

				Console.WriteLine("Key ron is conatined in the hashtable " + ht.Contains("are"));
				Console.WriteLine("Key randy is conatined in the hashtable " + ht.Contains("ran"));


			}
			catch (NonExistentKey<string> nek)
			{
				Console.WriteLine(nek.Message);
				Console.WriteLine(nek.StackTrace);
			}

		}
	}

	/// <summary>
	/// This is the main class which creates an object of LinkedHashTable 
	/// class and executes the methods implemented in the class inherited by the 
	/// table interface.
	/// </summary>

	class MainClass
	{	

		/// <summary>
		/// The main method which excutes some test cases by 
		/// </summary>
		public static void Main(string[] args)
		{
			/// <summary>
			/// This test's the put method by putting values in the table.
			/// It also test's for the Get method to get values of the keys.
			/// It also test's for any key not present in the table.
			/// </summary>
			LinkedHashTable<String,String> ht = (LinkedHashTable<String, String>)TableFactory<String,String>.Make<String, String>(4, 0.5);
			ht.Put("Joe", "Doe");
			ht.Put("Jane", "Brain");
			ht.Put("Chris", "Swiss");
			try
			{
				Console.WriteLine("           ");
				Console.WriteLine("Test Case 1");
				Console.WriteLine("           ");

				foreach (String first in ht)
				{
					Console.WriteLine(first + " -> " + ht.Get(first));
				}
				Console.WriteLine("=========================");
				Console.WriteLine("           ");
				Console.WriteLine("Test Case 2");
				Console.WriteLine("           ");

				ht.Put("Wavy", "Gravy");
				ht.Put("Chris", "Bliss");
				foreach (String first in ht)
				{
					Console.WriteLine(first + " -> " + ht.Get(first));
				}
				Console.WriteLine("=========================");

				Console.Write("Jane -> ");
				Console.WriteLine(ht.Get("Jane"));
				Console.Write("John -> ");
				Console.WriteLine(ht.Get("John"));


			}
			catch (NonExistentKey<String> nek)
			{
				Console.WriteLine(nek.Message);
				Console.WriteLine(nek.StackTrace);
			}
			Console.WriteLine("=========================");

			TestTable.test();
			Console.ReadLine();
		}
	}
}
