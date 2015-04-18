package infantrySquadCombat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import static infantrySquadCombat.Global.*;

public class Driver {
	
	public static void main(String[] args) throws InterruptedException {
		
		createAllConditions();
		createAllActions();
		createAllResponses();
		
		Unit sarge = new Unit("Sarge", "Infantry", "Sgt.");
		Unit griff = new Unit("Griff", "Infantry", "Pvt.");
		Unit simmons = new Unit("Simmons", "Infantry", "Pvt.");
		Unit church = new Unit("Church", "Infantry", "Lt.");
		Unit tucker = new Unit("Tucker", "Infantry", "Cpl.");
		Unit caboose = new Unit("Caboose", "Infantry", "Pvt.");
		
		List<Unit> reds = new ArrayList<>();
		reds.add(sarge);
		reds.add(simmons);
		reds.add(griff);
		Collections.sort(reds);
		
		for(Unit aUnit : reds) {
			aUnit.addResponse(takeCover);
			aUnit.addResponse(suppressingFire);
			aUnit.addResponse(advance);
			aUnit.addResponse(aimedFire);
			aUnit.addResponse(approach);
			aUnit.addResponse(wait);
		}
		
		Squad redTeam = new Squad("Red team", reds);
		List<Squad> redFaction = new ArrayList<>();
		redFaction.add(redTeam);
		
		List<Unit> blues = new ArrayList<>();
		blues.add(church);
		blues.add(caboose);
		blues.add(tucker);
		Collections.sort(blues);
		
		for(Unit aUnit : blues) {
			aUnit.addResponse(takeCover);
			aUnit.addResponse(suppressingFire);
			aUnit.addResponse(advance);
			aUnit.addResponse(aimedFire);
			aUnit.addResponse(approach);
			aUnit.addResponse(wait);
		}
		
		Squad blueTeam = new Squad("Blue team", blues);
		List<Squad> blueFaction = new ArrayList<>();
		blueFaction.add(blueTeam);
		
		Battlefield engagement = new Battlefield(15, "rocky", redFaction, blueFaction);
		engagement.commence(1);
	}
}
