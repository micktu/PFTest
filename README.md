# PF Test
Time of completion: approximately 11 hours.


The Unity version is 2019.2.9f1.


You can try a WebGL build [here](http://micktu.github.io/PFTest/index.html)


The goal was to build a robust, production-like system in a short timeframe, showcasing what I already know. I mostly succeeded, but I haven't achieved the quality I wanted as I ran out of time. The failure was spending the most time on a cool collision algo, only to fall back to the simpliest one.

I took a liberty of changing some of the game settings provided in order for the simulation to feel better.

## Architecture
I followed a few simple patterns.
* **State machines.** A bit simplified from what I would have done in a bigger project but still: having a clear control flow is nice.
* **Events.** The layers are connected through delegates and unaware of each other's internals. UI, for example, never calls into game logic.
* **Dependency injection**. No singletons, everything an object needs is fed directly into it on initialization.
* **Data separation**. The state data has its own structure making it easily serializable.
## Modules
* **GameManager** is our common friend that takes care of bootstrapping and a couple of global utilities (e.g., save-load).
* **MenuSystem** is a screen system that switches its state based on game state.
* **GameSystem** takes care of the outer gameplay logic.
* **EntitySystem** takes does collision and movement logic heavy-lifting.
## Entity System
Entities stored in a generic list of structures to make most use of good memory layout. Whenever they are accessed, ref arguments are used whenever possible; I'm not sure if it does any good in modern C#, though.

The collision system is pretty basic and inaccurate; initially I was aiming for speculative contacts collision, but ran out of time debugging things. Collision lookup is optimized by using a simple grid.
## Extra
It works fine, however, the random seed and sequence are not stored, so whenever you reload in the middle of spawn sequence, the result will be different for future spawns.

Time control also works.
## Things to improve
* **Rendering**: I was planning to make a custom dynamic mesh renderer to avoid game objects whatsoever, but I did not make it in time.
* **Visuals**: Again, I was planning to draw circles using distance fields in a shader, did not make it.
* **Code quality**: It got a bit messy in the end.
