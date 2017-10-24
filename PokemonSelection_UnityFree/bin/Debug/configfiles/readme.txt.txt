I detta dokument förklaras hur de olika konfigurationsfilerna för PokemonSelection ska användas.

PokemonDatabase1:
Varje pokemon följer syntaxen:

Namn;Typ1,Typ2;Attacktyp1,Attacktyp2,Attacktyp3,Attacktyp4;

Observera att Typ2 kan utelämnas, samt att mängden attacker inte har någon gräns, men bör hållas till max 4 för att det är så det är i spelen.


testTeams och evalTeams:
Synaxen är:

Team1Pokemon1;Team1Pokemon2;Team1Pokemon3;
Team2Pokemon1;Team2Pokemon2...etc

Ett kan bara innehålla tre pokemon, men mängden lag är obegränsat.


NEATConfig.config.xml

Det här är ett XML dokument och som sådant förklarar sig själv.
PopulationSize är den relevantaste variabeln och den avgör hur många genom som skall användas


ffConfig.txt
I detta dokument sätts variabler för feed forward nätverket på formen:

Försök per Träningstyp;generations;fitness;population;mutationschans;roulettselektionsvärde
Träningstyp anger vilken typ av träning som skall utföras, 1 innebär träning en viss mängd generationer, 2 inebär träning emot en viss fitness 
Generations anger hur mycket nätverket kommer att träna emot varje inmatat lag i testTeams
Fitness anger vilken fitness som söks
Population är mängden genom som genereras
Mutationschans anger hur stor chans det är att ett genom muterar i procent
Roulettevärdet anger hur många genom ett valt genom måste vinna emot för att faktiskt bli valt.
