namespace EnglishApp.Data;

public class FavoriteDeck
{
    public int UserId {get; set;}
    public int DeckId {get; set;}
    public DateTime CreateTime {get; set;} 
    
    public User User {get; set;}
    public Deck Deck {get; set;}
}