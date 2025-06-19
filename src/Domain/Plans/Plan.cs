using SharedKernel;

namespace Domain.Plans;
public class Plan : Entity
{

    private Plan()
    {
    }
    public Plan(Guid id, string name, DateTime creationTime, string createUser)
    {
        Id = id;
        Name = name;
        CreationTime = creationTime;
        CreateUser = createUser;
    }

    internal void SetElapse(DateRange elapse)
    {
        Elapse = elapse;
    }

    internal void SetCompleteTime(DateTime completeTime)
    {
        CompleteTime = completeTime;
    }

    public Guid Id { get; init; }
    public string Name { get; private set; }

    public DateRange? Elapse { get; private set; }

    public DateTime? CompleteTime { get; private set; }

    public DateTime CreationTime { get; init; }

    public string CreateUser { get; init; }
}
