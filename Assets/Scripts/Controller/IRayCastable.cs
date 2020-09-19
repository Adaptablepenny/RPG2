namespace RPG.Controller
{
    public interface IRayCastable
    {

        CursorType GetCursorType();
        bool HandleRaycast(PlayerController callingController);


    }
}