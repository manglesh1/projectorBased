using System;
using System.Collections.Generic;
using System.Text;
using Extensions;
using Games.GameState;
using Players;
using UnityEngine;

namespace Games.Word_Whack.Scripts
{
	public class WordWhackScoringLogic : MonoBehaviour
	{
		private List<string> _phraseCollection = new List<string>(97)
		{
			"A DIME A DOZEN", "BEATING AROUND THE BUSH", "BETTER LATE THAN NEVER", "BITE THE BULLET", "BREAK A LEG", "CALL IT A DAY", "CUT THEM SOME SLACK", "CUTTING CORNERS", "EASY DOES IT", "GETTING OUT OF HAND",
			"GET IT OUT OF YOUR SYSTEM", "BACK TO THE DRAWING BOARD", "HANG IN THERE", "HIT THE SACK", "IT'S NOT ROCKET SCIENCE", "OFF THE HOOK", "MISS THE BOAT", "NO PAIN NO GAIN", "ON THE BALL", "SO FAR SO GOOD",
			"SPEAK OF THE DEVIL", "THAT'S THE LAST STRAW", "THE BEST OF BOTH WORLDS", "DON'T GET BENT OUT OF SHAPE", "YOU CAN SAY THAT AGAIN", "A PENNY FOR YOUR THOUGHTS", "A PERFECT STORM", "ACTIONS SPEAK LOUDER THAN WORDS", "ADD INSULT TO INJURY", "BARKING UP THE WRONG TREE",
			"WIN BY A LONG SHOT", "BEAT A DEAD HORSE", "BEST THING SINCE SLICED BREAD", "AN ARM AND A LEG", "CROSS YOUR FINGERS", "DON'T CRY OVER SPILLED MILK", "CUT THE MUSTARD", "DOWN TO THE WIRE", "EVERYTHING BUT THE KITCHEN SINK", "A FAR CRY FROM HOME",
			"FEELING A BIT UNDER THE WEATHER", "FIT AS A FIDDLE", "IT'S A FIXER UPPER", "FLASH IN THE PAN", "FOR WHAT IT'S WORTH", "GETTING COLD FEET", "GET IT OFF YOUR CHEST", "THE BENEFIT OF THE DOUBT", "GOING DOWN IN FLAMES", "GO THE EXTRA MILE",
			"HIT BELOW THE BELT", "IN A NUT SHELL", "IN HOT WATER", "THE HEAT OF THE MOMENT", "IT'S A SMALL WORLD", "IT'S ANYONE'S CALL", "KEEP YOUR CHIN UP", "KICK THE BUCKET", "IT'S THE LAST STRAW", "LET THE CAT OUT OF THE BAG",
			"LET YOUR HAIR DOWN", "BULL IN A CHINA SHOP", "TO MAKE A LONG STORY SHORT", "FINDING NEEDLE IN A HAY STACK", "OFF THE TOP OF MY HEAD", "ON CLOUD NINE", "ON THIN ICE", "ONCE IN A BLUE MOON", "OUT OF THE BLUE", "OVER THE MOON",
			"A PIECE OF CAKE", "PULLING YOUR LEG", "PUT ALL YOUR EGGS IN ONE BASKET", "DON'T RAIN ON MY PARADE", "RIDE OUT THE STORM", "RIGHT OFF THE BAT", "RUN OUT OF STEAM", "SAVED BY THE BELL", "SEEING EYE TO EYE", "ON THE FENCE",
			"SPILL THE BEANS", "TAKE IT WITH A GRAIN OF SALT", "THE BALL IS IN YOUR COURT", "THE HARDER THEY FALL", "THE DEVIL IS IN THE DETAILS", "EARLY BIRD GETS THE WORM", "THE ELEPHANT IN THE ROOM", "THE WHOLE NINE YARDS", "NO SUCH THING AS A FREE LUNCH", "THROW IN THE TOWEL",
			"TIME FLIES WHEN YOU'RE HAVING FUN", "ADDING FUEL TO THE FIRE", "UP IN THE AIR", "WHEN IT RAINS IT POURS", "A CHIP ON YOUR SHOULDER", "BURST YOUR BUBBLE", "BETWEEN A ROCK AND A HARD PLACE", "CLOSE BUT NO CIGAR", "CURIOSITY KILLED THE CAT", "CUT TO THE CHASE",
			"DON'T JUDGE A BOOK BY IT'S COVER", "DOWN AND OUT", "DOWN TO EARTH", "DRAWING A BLANK", "DRIVE ME NUTS", "DROPPING LIKE FLIES", "DEER IN HEADLIGHTS", "DON'T SWEAT IT", "DOWN IN THE DUMPS", "DROP THE BALL",
			"EASY AS PIE", "JUMP THE SHARK", "ELVIS HAS LEFT THE BUILDING", "EVERY CLOUD HAS A SILVER LINING", "FIGHT FIRE WITH FIRE", "FISH OUT OF WATER", "FOOL ME ONCE SHAME ON YOU", "FACE THE MUSIC", "WON FAIR AND SQUARE", "FROG IN YOUR THROAT",
			"GIVE A MAN A FISH", "DOWN THE RABBIT HOLE", "GO FOR BROKE", "OUT ON A LIMB", "GRASS IS ALWAYS GREENER", "GET A GRIP", "GET OFF YOUR HIGH HORSE", "GET THE BALL ROLLING", "GIVE ME A HAND", "GLUED TO THEIR SEATS",
			"GOT A KICK OUT OF IT", "GOT OFF ON THE WRONG FOOT", "HAPPY AS A CLAM", "HARD PILL TO SWALLOW", "HEAD OVER HEELS", "HIGH AND DRY", "HOUSTON WE HAVE A PROBLEM", "HOW DO YOU LIKE THEM APPLES", "HUNG OUT TO DRY", "I SPY WITH MY LITTLE EYE",
			"IF IT AIN'T BROKE DON’T FIX IT", "IN A PICKLE", "IN ONE FELL SWOOP", "NOT ALL IT'S CRACKED UP TO BE", "IT'S NOT BRAIN SURGERY", "IN THE NICK OF TIME", "IN FOR A PENNY IN FOR A POUND", "JACK OF ALL TRADES MASTER OF NONE", "JUMP THE GUN", "JIG IS UP",
			"JUMP ON THE BAND WAGON", "JUST WHAT THE DOCTOR ORDERED", "JUMPED OUT OF MY SKIN", "KEEP YOUR SHIRT ON", "KNOCK YOUR SOCKS OFF", "KNOW THE ROPES", "KILL TWO BIRDS WITH ONE STONE", "KNOCK YOURSELF OUT", "LET BYGONES BE BYGONES", "LIKE SHOOTING FISH IN A BARREL",
			"LIKE A MOTH TO A FLAME", "LONG TIME NO SEE", "MONEY DOESN'T GROW ON TREES", "MISERY LOVES COMPANY", "MONKEY SEE MONKEY DO", "MUSIC TO MY EARS", "NOT THE SHARPEST TOOL IN THE SHED", "NOTHING TO SNEEZE AT", "OPEN A CAN OF WORMS", "OVER THE TOP",
			"PAINT THE TOWN RED", "PLAYING FOR KEEPS", "POT CALLING THE KETTLE BLACK", "PUT A SOCK IN IT", "PAR FOR THE COURSE", "ON PINS AND NEEDLES", "QUICK ON THE DRAW", "QUIET AS A MOUSE", "QUIT WHILE YOU'RE AHEAD", "RAIN ON YOUR PARADE",
			"RAINING CATS AND DOGS", "ROLL WITH THE PUNCHES", "ROME WAS NOT BUILT IN A DAY", "RUNNING ON FUMES", "SHORT END OF THE STICK", "SHOT IN THE DARK", "SITTING DUCK", "SLOW AND STEADY WINS THE RACE", "STICK A FORK IN IT", "SWINGING FOR THE FENCES",
			"TEAMWORK MAKES THE DREAMWORK", "THE PLOT THICKENS", "THERE'S NO I IN TEAM", "TOO MANY COOKS IN THE KITCHEN", "TWO PEAS IN A POD", "WHAT GOES UP MUST COME DOWN", "WHERE THE RUBBER HITS THE ROAD", "WILD GOOSE CHASE", "CAN'T TEACH AN OLD DOG NEW TRICKS", "YOU ARE WHAT YOU EAT",
			"YOU SNOOZE YOU LOSE", "YOU CAN COUNT ON ME"
		};

		private Dictionary<char, WordWhackCharacterStateEnum> _characterMap;

		private string _currentPhrase;

		private Stack<WordWhackGameHistoryState> _phraseHistoryStack;

		private int _currentPlayerIndex;

		[Header("State")]
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[Header("Events")]
		[SerializeField]
		private WordWhackEventsSO wordWhackEvents;

		public string CurrentPhrase => _currentPhrase;

		private void OnDisable()
		{
		}

		private void OnEnable()
		{
			_phraseHistoryStack = new Stack<WordWhackGameHistoryState>();
			ResetCharacterMap();
		}

		private void AddHistory()
		{
			_phraseHistoryStack.Push(new WordWhackGameHistoryState
			{
				CharacterMap = _characterMap.SimpleJsonClone(),
				PlayersTurn = gameState.CurrentPlayer
			});
		}

		public bool CharacterFound(char character)
		{
			return _characterMap.ContainsKey(character) && _characterMap[character] == WordWhackCharacterStateEnum.Found;
		}

		public string GetMaskedPhrase()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _currentPhrase.Length; i++)
			{
				char c = _currentPhrase[i];
				if (_characterMap.ContainsKey(c) && _characterMap[c] != WordWhackCharacterStateEnum.Found)
				{
					stringBuilder.Append('_');
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public void Initialize()
		{
			_phraseHistoryStack.Clear();
			_currentPlayerIndex = 0;
			SetPhrase();
			SetCurrentPlayer();
			AddHistory();
		}

		public bool HasPlayerWon()
		{
			bool result = true;
			string currentPhrase = _currentPhrase;
			foreach (char c in currentPhrase)
			{
				if (c != ' ' && c != '\'' && _characterMap[c] != WordWhackCharacterStateEnum.Found)
				{
					return false;
				}
			}
			return result;
		}

		public void Miss()
		{
			AddHistory();
			NextPlayer();
		}

		public void NextPlayer()
		{
			if (_currentPlayerIndex >= playerState.CurrentPlayerNames.Count - 1)
			{
				_currentPlayerIndex = 0;
			}
			else
			{
				_currentPlayerIndex++;
			}
			SetCurrentPlayer();
		}

		public void RecordCharacter(char character)
		{
			AddHistory();
			_characterMap[character] = ((_currentPhrase.IndexOf(character.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0) ? WordWhackCharacterStateEnum.Found : WordWhackCharacterStateEnum.NotFound);
		}

		private void ResetCharacterMap()
		{
			if (_characterMap == null)
			{
				_characterMap = new Dictionary<char, WordWhackCharacterStateEnum>();
				for (char c = 'A'; c <= 'Z'; c = (char)(c + 1))
				{
					if (!_characterMap.ContainsKey(c))
					{
						_characterMap.Add(c, WordWhackCharacterStateEnum.Hidden);
					}
				}
			}
			else
			{
				for (char c2 = 'A'; c2 <= 'Z'; c2 = (char)(c2 + 1))
				{
					_characterMap[c2] = WordWhackCharacterStateEnum.Hidden;
				}
			}
		}

		private void SetCurrentPlayer()
		{
			gameState.CurrentPlayer = playerState.CurrentPlayerNames[_currentPlayerIndex];
		}

		private void SetPhrase()
		{
			ResetCharacterMap();
			int index = UnityEngine.Random.Range(0, _phraseCollection.Count - 1);
			_currentPhrase = _phraseCollection[index].Trim().ToUpper();
		}

		public bool SolveAttempt(string phrase)
		{
			AddHistory();
			return _currentPhrase.Replace("'", string.Empty).Trim() == phrase.Replace("'", string.Empty).Trim().ToUpper();
		}

		public void Undo()
		{
			if (_phraseHistoryStack.Count == 0)
			{
				return;
			}
			WordWhackGameHistoryState wordWhackGameHistoryState = _phraseHistoryStack.Pop();
			_characterMap = wordWhackGameHistoryState.CharacterMap.SimpleJsonClone();
			_currentPlayerIndex = playerState.CurrentPlayerNames.IndexOf(wordWhackGameHistoryState.PlayersTurn);
			gameState.CurrentPlayer = wordWhackGameHistoryState.PlayersTurn;
			foreach (KeyValuePair<char, WordWhackCharacterStateEnum> item in _characterMap)
			{
				switch (item.Value)
				{
				case WordWhackCharacterStateEnum.Hidden:
					wordWhackEvents.RaiseResetCharacter(item.Key);
					break;
				case WordWhackCharacterStateEnum.Found:
					wordWhackEvents.RaiseOnCharacterFound(item.Key);
					break;
				case WordWhackCharacterStateEnum.NotFound:
					wordWhackEvents.RaiseOnCharacterNotFound(item.Key);
					break;
				}
			}
		}
	}
}
