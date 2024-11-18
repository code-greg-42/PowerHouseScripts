# PowerHouse: Aim Practice

PowerHouse: Aim Practice functions as a simple aim trainer for those wishing to improve their mouse clicking abilities for shooters and aim-related games. It also includes 3 fun abilities to add a layer of decision-making and strategy not usually seen in aim trainers, but seen frequently in modern shooting games.

## Play Now!

[Play PowerHouse: Aim Practice on Unity Play](https://play.unity.com/en/games/4a46ca87-b84e-411a-969b-a9b17a313efb/powerhouse11)

## Features

- Dynamic health of enemies: Each enemy takes 3 shots to kill, and begins healing back up if not shot for the last 5 seconds. The color of the enemy changes based on the current health total
- Sporadic movement of enemies: Each enemy has its own movement interval, and will choose a random direction to move in every so often based on the movement interval. This leads to better aim practice as the movements are somewhat unpredictable.
- Abilities: The 3 abilities each cause different interactions with the enemies. Golden Gun causes enemies to be oneshotted (and show a different death effect). Slow Time causes all the enemies to dramatically slow their movement speed for the next 3 seconds. Force Pull drags all current enemies to the center of the screen near the floor, and pause there for 1 second. Each has its own individual cooldown, and they can be combo'd together for impressive results.
- Point System: Bonus points are awarded for clearing a wave ahead of the timer. It is multiplicative and rewards more points for more remaining time, as well as more points for clearing higher waves with time remaining. If a leaderboard system is added, it will be sorted first by Waves defeated, then by total points.

## How to Play

Simply click the objects on the screen as fast as you can! As stated above, they take 3 shots to kill, but will heal from low health every 5 seconds they are not shot (from red to yellow, then yellow to green after another 5 seconds). Once all the objects are exploded, a new wave will spawn with an increasing amount of enemies. Use the following keybinds for abilities:
- E: Golden Gun (1 shot kills for 5 seconds)
- F: Slow Time (duration of 3 seconds)
- C: Force Pull

## Tips
- Force pull and golden gun work very well together. Try saving force pull until golden gun is ready.
- Sometimes force pull is required to get an object centered from the front of the screen. Try using force pull early in a wave so that it's ready when the timer is almost up.
- Pro Tip: To really rack up the points, try using slow time to start a round, and when the duration is up, force pull combo'd with golden gun to clear up the rest and clear a wave really quick. Warning: The next wave will be hard as you won't start with any cooldowns up!

## Development

As my first microgame with Unity, I wanted to start with something simple, but still fun and playable. I especially enjoyed working with the physics interactions of the different abilities, mainly Slow Time and Force Pull. I encountered plenty of trouble in trying to get the enemy objects to move the way I wanted, and learned a lot in the process. I hope to carry this knowledge into future games I create or work on!

## Credits

**Unity Learn Junior Programmer Pathway**: Following this course has given lots of insight and been tremendously helpful in creating this game.

## Feedback

I would love to hear any feedback, advice, or general recommendations for creating games as I will be continuing to learn Unity and Game Development, and aspire to join the Game Development Industry. Please use the [GitHub Issues](https://github.com/code-greg-42/PowerHouseScripts/issues) for this repository to contact me. I appreciate any and all input. Thanks!
