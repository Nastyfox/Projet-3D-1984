public interface IStateNPC
{
    void OnEntry(NPCStateController controller);

    void OnUpdate(NPCStateController controller);

    void OnExit(NPCStateController controller);
}
