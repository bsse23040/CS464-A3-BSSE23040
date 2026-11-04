# CS464 Assignment 03 · Mohsin Raza Ojla · BSSE23040

- **Video (60–120 s):** VIDEO_LINK_HERE
- **Started from:** my Assignment 02 repo, https://github.com/bsse23040/CS464-A2-BSSE23040. I copied it with its history, so the five levels, the controller and the camera are the same ones, and everything for this assignment is added on top of them.
- **Controls:** `W A S D` or the arrow keys to move, `Space` to jump. They come from the Input System's default actions, `Move` and `Jump`.
- **To win a level:** collect the number of coins shown at the top left (7, 12, 6, 7 and 9 in levels 1 to 5), then stand on the green goal. With too few coins the goal only tells you how many are missing. You have 3 lives with 3 hearts each, and falling off a level costs a life.

Made and tested in Unity 6000.6.2f1.

## Levels

| Level | Screenshot | New idea | Combined with |
|---|---|---|---|
| Level01 | <img src="Docs/a3/level01.png" width="320"> | Coins, the goal and a bounce pad that throws you onto a ledge with two coins. | Nothing hurts yet. The only danger is the 2.5 m gap before the goal, and a checkpoint sits in front of it. |
| Level02 | <img src="Docs/a3/level02.png" width="320"> | Spikes, which cost one heart at a time. | A coin trail that leads over spikes, and a reward room with spikes in its doorway that you have to jump. |
| Level03 | <img src="Docs/a3/level03.png" width="320"> | A guard that patrols, chases when you come close and gives up when you get away. | Spikes right beside its route, so dodging the guard can push you into them. |
| Level04 | <img src="Docs/a3/level04.png" width="320"> | The crusher, which warns before it strikes. It is first met alone on the first platform, with a coin on the pad. | A guard that walks beside a second crusher on the last platform, with the two 2.5 m jumps and the stairs between. |
| Level05 | <img src="Docs/a3/level05.png" width="320"> | Two speed pickups 6 m apart, so the second one is picked up while the first is still running. | Everything before it: a ping-pong guard, spikes, a crusher that fills the ramp room, the narrow bridge, and the 2.8 m jump at the end. |

## The numbers

| Number | Value | Why this value |
|---|---|---|
| Patrol speed | 2 m/s | The player runs at 5 m/s, so a guard walks at 40% of that. It is slow enough to read its route and walk past it. |
| Chase range | 6 m, gives up at 9 m. Chase speed 3.5 m/s | One jump carries about 5 m, so 6 m means you cannot jump past a guard without it seeing you. It gives up 3 m further out so it does not flicker on and off at one edge. Chasing at 3.5 m/s is 70% of my run speed, so I can always outrun it. |
| Knockback | 8 with `ForceMode.Impulse` (the crusher uses 10), and no steering for 0.25 s | On the 1 kg player an impulse of 8 is a speed of 8 m/s for 0.25 s, about 2 m. I measured 1.9 m in play, which is enough to clear the 2 m spike patch. |
| I-frames | 1 s | After a hit it takes 0.25 s of knockback plus a short walk to get out of a 2 m patch, which is under a second. Long enough to get clear, short enough that standing in spikes still costs a heart every second. |
| Power-up duration | 5 s at x1.6 speed (8 m/s) | That is about 40 m of running. It covers the two door frames and the pickups 6 m apart in level 5, but not the whole level, so the boost is a help and not the normal speed. |
| Crusher warning | 0.7 s (rest 2.4 s, strike 0.4 s) | Reaction time is about 0.25 s and you also have to move, so 0.7 s is a fair warning. 0.2 s would feel like a trick. |
| Bounce pad | 9 m/s up with `ForceMode.VelocityChange` | The apex is v squared over 2g, 4.1 m. The ledge is 3 m high, so you clear it with a bit of room. |
| Respawn | 0.6 s delay, 1.5 s protection | The delay lets the knockback of the killing hit finish before the new life starts, and the protection stops a spawn point from killing twice. |

## Short answers

**1. Why does a trigger need a Rigidbody on at least one of the two objects?**

Unity only checks for overlaps when at least one of the two colliders belongs to a Rigidbody. Two colliders with no Rigidbody are both treated as static, so they never get tested against each other and nothing fires. In my game the player has the Rigidbody, so coins, spikes, the goal and the checkpoints (none of which have one) all fire. The guards move by script, so they get a kinematic Rigidbody of their own: a collider that moves should have one.

**2. Which ForceMode do your knockback and your Part A force use, and why each?**

The knockback uses `Impulse`, because it is a single kick and I want the mass to count: 8 on a 1 kg player is 8 m/s. It is called once, from `OnTriggerStay`, only when the hit lands. The bounce pad uses `VelocityChange`, because I want the height to depend only on the number and not on the player's mass (9 m/s gives 4.1 m), and it kicks once from `OnTriggerEnter`. My controller sets the sideways speed every physics step, so the pad pushes straight up, and the knockback stops the steering for 0.25 s so it is not wiped.

**3. What does your code do when a second power-up is picked up while one is running?**

It refreshes it. `BoostSpeed` stops the running coroutine and starts a new one, so the 5 seconds start again and the speed does not stack. Two pickups 3 seconds apart would end at 8 seconds. In level 5 the second pickup takes the boost from about 2.2 seconds left back up to 5.

**4. Why can `==` fail as an arrival check, and when does it work?**

`Vector3 ==` only counts positions that are within about 0.00001 m. If I move an object by hand with a step like 3 cm per frame it never lands that close, so it stops just short, overshoots and turns round for ever and never arrives. It works right after `Vector3.MoveTowards`, because that lands exactly on the target when it is within one step. My guard still checks arrival by distance, with `arriveDistance` 0.1 m, which is bigger than one step (2 m/s is about 3 cm a frame), so it also works for the chase and the walk back.
