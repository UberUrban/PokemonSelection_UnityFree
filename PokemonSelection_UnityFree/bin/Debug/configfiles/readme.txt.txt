I detta dokument f�rklaras hur de olika konfigurationsfilerna f�r PokemonSelection ska anv�ndas.

PokemonDatabase1:
Varje pokemon f�ljer syntaxen:

Namn;Typ1,Typ2;Attacktyp1,Attacktyp2,Attacktyp3,Attacktyp4;

Observera att Typ2 kan utel�mnas, samt att m�ngden attacker inte har n�gon gr�ns, men b�r h�llas till max 4 f�r att det �r s� det �r i spelen.


testTeams och evalTeams:
Synaxen �r:

Team1Pokemon1;Team1Pokemon2;Team1Pokemon3;
Team2Pokemon1;Team2Pokemon2...etc

Ett kan bara inneh�lla tre pokemon, men m�ngden lag �r obegr�nsat.


NEATConfig.config.xml

Det h�r �r ett XML dokument och som s�dant f�rklarar sig sj�lv.
PopulationSize �r den relevantaste variabeln och den avg�r hur m�nga genom som skall anv�ndas


ffConfig.txt
I detta dokument s�tts variabler f�r feed forward n�tverket p� formen:

F�rs�k per Tr�ningstyp;generations;fitness;population;mutationschans;roulettselektionsv�rde
Tr�ningstyp anger vilken typ av tr�ning som skall utf�ras, 1 inneb�r tr�ning en viss m�ngd generationer, 2 ineb�r tr�ning emot en viss fitness 
Generations anger hur mycket n�tverket kommer att tr�na emot varje inmatat lag i testTeams
Fitness anger vilken fitness som s�ks
Population �r m�ngden genom som genereras
Mutationschans anger hur stor chans det �r att ett genom muterar i procent
Roulettev�rdet anger hur m�nga genom ett valt genom m�ste vinna emot f�r att faktiskt bli valt.
