package infantrySquadCombat;
import java.util.ArrayList;
import java.util.List;

public class Battlefield {

	private List<Squad> factionASquads;
	private List<Unit> factionAUnits;
	
	private List<Squad> factionBSquads;
	private List<Unit> factionBUnits;
	
	private List<Unit> allCombatants;
	
	private List<Integer> terrainCover;
	
	public Battlefield (int size, String terrainType, List<Squad> allASquads, List<Squad> allBSquads) {
		factionASquads = allASquads;
		factionBSquads = allBSquads;
		
		allCombatants = new ArrayList<>();
		factionAUnits = new ArrayList<>();
		factionBUnits = new ArrayList<>();
		
		for(Squad aSquad : factionASquads) {
			for(Unit aUnit : aSquad.getAllFriendlies()) {
				aUnit.setStartPosition(0);
				factionAUnits.add(aUnit);
				allCombatants.add(aUnit);
			}
		}
		
		for(Squad aSquad : factionBSquads) {
			for(Unit aUnit : aSquad.getAllFriendlies()) {
				aUnit.setStartPosition(size);
				factionBUnits.add(aUnit);
				allCombatants.add(aUnit);
			}
		}
		
		for(Squad aSquad : factionASquads) {
			List<Unit> allEnemies = new ArrayList<>();
			allEnemies.addAll(factionBUnits);
			aSquad.enemyReinforces(allEnemies);
		}
		
		for(Squad aSquad : factionBSquads) {
			List<Unit> allEnemies = new ArrayList<>();
			allEnemies.addAll(factionAUnits);
			aSquad.enemyReinforces(allEnemies);
		}
		
		terrainCover = new ArrayList<>(size);
		
		for(int i=0; i < size; i++) {
			int min;
			int max;
			switch (terrainType) {
				case "open":
					min = 0;
					max = 2;
					break;
				case "rocky":
					min = 3;
					max = 8;
					break;
				default:
					min = 0;
					max = 5;
			}
			int randomCover = min + (int)(Math.random() * (max - min));
			terrainCover.add(randomCover);
		}
	}
	
	public List<Integer> getCoverAvailable() {
		return terrainCover;
	}
	
	public void commence (int maxTurns) throws InterruptedException {
		String factionASquadNames = "";
		for(Squad aSquad : factionASquads) {
			if(factionASquadNames == "") {
				factionASquadNames += aSquad.getName();
				continue;
			}
			factionASquadNames += ", " + aSquad.getName();
		}
		
		String factionBSquadNames = "";
		for(Squad aSquad : factionBSquads) {
			if(factionBSquadNames == "") {
				factionBSquadNames += aSquad.getName();
				continue;
			}
			factionBSquadNames += ", " + aSquad.getName();
		}
		
		String outputString = "--- " + factionASquadNames + " vs. " + factionBSquadNames + " ---";
		
		System.out.println(outputString);
		
		for(Unit who : allCombatants) {
			who.start();
		}

		for (Unit who : allCombatants) {
			who.join();
		}
	}
}
