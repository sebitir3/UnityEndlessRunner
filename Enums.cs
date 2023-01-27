public enum State
{
    Run,
    Slide,
    Jump
}

//layers -- collisions
public enum Layers
{
    //built in layers
    Default,
    TransparentFX,
    IgnoreRaycast,
    Water = 4,
    UI,
    //user layers
    Player = 8,
    Obstacle
}