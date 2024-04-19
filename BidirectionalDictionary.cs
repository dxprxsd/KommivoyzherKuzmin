using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*
Имплементация класса для bidirectional map (двустороння карта). Суть в том, что мы храним связь в оба направления между
буквами и индексами. Соответственно, имея индекс, мы можем получить букву, и наоборот.

Класс используется в каждом узле (то есть для каждой матрицы) два раза:
rowToLetter - экземпляр этого класса, в котором мы содержим связь букв <-> индексов для строк матрицы
colToLetter - экземпляр этого класса, в котором мы содержим связь букв <-> индексов для столбцов матрицы
*/
public class BidirectionalDictionary
{
    private Dictionary<int, char> indexToLetter = new Dictionary<int, char>();
    private Dictionary<char, int> letterToIndex = new Dictionary<char, int>();
    //1 конструктор
    public BidirectionalDictionary(char[] letters)
    {
        for (int i = 0; i < letters.Length; i++)
        {
            this.Add(i, letters[i]);
        }
    }
    //2 конструктор
    public BidirectionalDictionary(Dictionary<int, char> indexToLetter, Dictionary<char, int> letterToIndex)
    {
        this.indexToLetter = indexToLetter;
        this.letterToIndex = letterToIndex;
    }

    // private, потому что в нашем решении не задумано использование .Add()
    private void Add(int index, char letter) 
    {
        if (indexToLetter.ContainsKey(index) || letterToIndex.ContainsKey(letter))
        {
            throw new ArgumentException("В двунаправленный словарь были добавлены дубликатная буква города или индекс.");
        }

        indexToLetter[index] = letter;
        letterToIndex[letter] = index;
    }
    //из индекса в букву
    public char GetLetter(int index)
    {
        return indexToLetter[index];
    }

    // из буквы в индекс
    public int GetIndex(char letter)
    {
        return letterToIndex[letter];
    }

    public bool CheckIfLettersExists(char letter)
    {
        return letterToIndex.ContainsKey(letter);
    }

    private void RemoveByLetterWithoutMoving(char letter)
    {
        int index = letterToIndex[letter];
        letterToIndex.Remove(letter);
        indexToLetter.Remove(index);
    }

    private void RemoveByIndexWithoutMoving(int index)
    {
        char letter = indexToLetter[index];
        indexToLetter.Remove(index);
        letterToIndex.Remove(letter);
    }

    public int GetCount()
    {
        return indexToLetter.Count;
    }

    // letter - буква, которую мы удаляем
    public BidirectionalDictionary RemoveByLetterWithAdjustments(char letter)
    {
        int removedIndex = this.GetIndex(letter);
        Dictionary<int, char> newIndexToLetter = new Dictionary<int, char>();
        Dictionary<char, int> newLetterToIndex = new Dictionary<char, int>();


        foreach (var item in this.indexToLetter)
        {
            // Пропускаем пару ключ-значение, если значение - это буква letter, которую нам надо удалить
            if (item.Value == letter)
            {
                continue;
            }

            int newIndex = item.Key; // новый индекс = старому индексу
            if (item.Key > removedIndex) // если индекс > индекс удаляемой строки, то нужно этот индекс сдвинуть назад на 1
            {
                newIndex -= 1;
            }

            // Заполняем оба наших словаря (оба направления)
            newIndexToLetter[newIndex] = item.Value;
            newLetterToIndex[item.Value] = newIndex;
        }

        // создаем объект класса BidirectionalDictionary и возвращаем его
        BidirectionalDictionary newBidirectionalDictionary = new BidirectionalDictionary(newIndexToLetter, newLetterToIndex);
        return newBidirectionalDictionary;
    }

    static public BidirectionalDictionary CreateNewCopy(BidirectionalDictionary item)
    {
        Dictionary<int, char> newIndexToLetter = new Dictionary<int, char>();
        Dictionary<char, int> newLetterToIndex = new Dictionary<char, int>();

        foreach (var pair in item.indexToLetter)
        {
            newIndexToLetter[pair.Key] = pair.Value;
        }

        foreach (var pair in item.letterToIndex)
        {
            newLetterToIndex[pair.Key] = pair.Value;
        }

        BidirectionalDictionary newBidirectionalDictionary = new BidirectionalDictionary(newIndexToLetter, newLetterToIndex);

        return newBidirectionalDictionary;
    }
}