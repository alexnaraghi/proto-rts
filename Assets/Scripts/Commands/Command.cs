
public class Command 
{
    public int TeamNumber
    {
        get; protected set;
    }
    
    public virtual void Execute() { }
    
    public virtual void Undo() {}
}