package infantrySquadCombat;
import static infantrySquadCombat.Global.*;
import static org.junit.Assert.*;

import java.util.ArrayList;
import java.util.List;

import org.junit.Before;
import org.junit.Test;

public class UnitTest {
		
	public Unit sarge;
	public Unit church;
	
	public Squad redTeam;
	public Squad blueTeam;
	
	public Response chosenResponse;
	List<Unit> allFriendlies;
	List<Unit> allFoes;

	@Before
	public void setUp() throws Exception {
		
		createAllActions();
		createAllConditions();
		createAllResponses();
		
		sarge = new Unit("Sarge", "Infantry", "Sgt.");
		church = new Unit("Church", "Infantry", "Liutenant");
		
		for(Response aResponse : allResponses) {
			sarge.addResponse(aResponse);
			//church.addResponse(aResponse);
		}
		
		List<Unit> reds = new ArrayList<>();
		reds.add(sarge);
		redTeam = new Squad("Red team", reds);
		
		List<Unit> blues = new ArrayList<>();
		blues.add(church);
		blueTeam = new Squad("Blue team", blues);
		
		sarge.setStartPosition(0);
		
		allFriendlies = new ArrayList<>();
		allFriendlies.add(sarge);
		
		allFoes = new ArrayList<>();
		allFoes.add(church);
	}
	
	@Test
	public void unitRefresh() throws InterruptedException {
		sarge.refresh(allFriendlies, allFoes);
	}
	
	@Test
	public void getAllConditions() throws InterruptedException {
		List<Condition> allConditionsCopy = new ArrayList<>(allConditions);
		
		int conditionsCode = sarge.saveAllConditionsIdCode();
		getListOfAllConditionsFromCode(conditionsCode);
	}

	@Test
	public void unitTakesCoverUnderFire() throws InterruptedException {
		System.out.println("Take cover:");
		unitRefresh();
		
		sarge.underFire = true;
		church.setStartPosition(4);
		sarge.cover = 6;
		sarge.inCover = 0;
		
		int conditionsCode = sarge.saveAllConditionsIdCode();
		
		chosenResponse = sarge.respond();
		assertTrue(chosenResponse.equals(takeCover));
		System.out.println("");
	}
	
	@Test
	public void unitReturnsAimedFire() throws InterruptedException {
		System.out.println("Aimed fire:");
		//sarge.underFire = true;
		unitRefresh();
		
		church.setStartPosition(4);
		church.suppressed = true;
		sarge.suppressed = false;
		sarge.cover = 6;
		sarge.inCover = 2;

		int conditionsCode = sarge.saveAllConditionsIdCode();
		getListOfAllConditionsFromCode(conditionsCode);
		
		chosenResponse = sarge.respond();
		assertTrue(chosenResponse.equals(aimedFire));
		System.out.println("");
	}
	
	@Test
	public void unitAdvances() throws InterruptedException {
		System.out.println("Advance:");
		//sarge.underFire = true;
		//sarge.cover = 6;
		//sarge.inCover = 2;
		unitRefresh();
		
		church.setStartPosition(5);
		church.suppressed = true;
		sarge.suppressed = false;
		sarge.threatLevel = 5;
		sarge.courage = 10;
		sarge.panic = 0;

		int conditionsCode = sarge.saveAllConditionsIdCode();

		chosenResponse = sarge.respond();
		assertTrue(chosenResponse.equals(advance));
		System.out.println("");
	}
	
	@Test
	public void unitDirectsSuppressingFire() throws InterruptedException {
		System.out.println("Suppressing fire:");
		//sarge.underFire = true;
		//sarge.cover = 6;
		//sarge.inCover = 2;
		//sarge.pinned = false;
		//sarge.threatLevel = 5;
		//sarge.courage = 10;
		//sarge.panic = 0;
		unitRefresh();
		
		church.setStartPosition(5);
		church.suppressed = false;

		int conditionsCode = sarge.saveAllConditionsIdCode();

		
		chosenResponse = sarge.respond();
		assertTrue(chosenResponse.equals(suppressingFire));
		System.out.println("");
	}
	
	@Test
	public void unitReloadsAfterFiring() throws InterruptedException {
		System.out.println("Fire three times then reload:");
		//sarge.underFire = true;
		//sarge.cover = 6;
		//sarge.inCover = 2;
		//sarge.pinned = false;
		//sarge.threatLevel = 5;
		//sarge.courage = 10;
		//sarge.panic = 0;
		unitRefresh();
		
		church.setStartPosition(4);
		church.suppressed = true;
		sarge.suppressed = false;
		sarge.cover = 6;
		sarge.inCover = 2;
		sarge.armUnit(new Armament("test rifle", "", 0, 0, 2, 1, 5, 3));

		int conditionsCode = sarge.saveAllConditionsIdCode();
		getListOfAllConditionsFromCode(conditionsCode);
		
		chosenResponse = sarge.respond();
		assertTrue(chosenResponse.equals(aimedFire));

		int ammo = sarge.equippedWeapon.getRoundsRemaining();
		assertTrue(ammo == 2);
		
		chosenResponse = sarge.respond();
		assertTrue(chosenResponse.equals(aimedFire));

		ammo = sarge.equippedWeapon.getRoundsRemaining();
		assertTrue(ammo == 1);
		
		chosenResponse = sarge.respond();
		assertTrue(chosenResponse.equals(aimedFire));
		
		ammo = sarge.equippedWeapon.getRoundsRemaining();
		assertTrue(ammo == 3);
		System.out.println("");
	}
	
	@Test
	public void unitRegroups() throws InterruptedException {
		System.out.println("Regroup:");
		Unit grif = new Unit("Grif", "Infantry", "Pvt.");
		List<Unit> reinforcement = new ArrayList<>();
		reinforcement.add(grif);
		redTeam.reinforce(reinforcement);
		allFriendlies.add(grif);
		
		//sarge.underFire = true;
		//sarge.cover = 6;
		//sarge.inCover = 2;
		//sarge.pinned = false;
		//sarge.threatLevel = 5;
		//sarge.courage = 10;
		//sarge.panic = 0;
		unitRefresh();
		
		grif.setStartPosition(1);
		church.setStartPosition(8);
		church.suppressed = true;
		sarge.courage = 0;
		sarge.cover = 0;
		sarge.suppressed = false;
		
		int sargePosition = sarge.getPosition();
		assertTrue(sargePosition == 0);

		int conditionsCode = sarge.saveAllConditionsIdCode();
		
		chosenResponse = sarge.respond();
		assertTrue(chosenResponse.equals(regroup));
		
		sargePosition = sarge.getPosition();
		int grifPosition = grif.getPosition();
		assertTrue(sargePosition == grifPosition);
		
		System.out.println("");
	}
	
/*	@Test
	public void displayResponses() throws InterruptedException {
		System.out.println("Initial order:");
		sarge.displayResponseOrder();
		
		unitTakesCoverUnderFire();
		unitReturnsAimedFire();
		//unitReturnsAimedFire();
		
		System.out.println("Final order:");
		sarge.displayResponseOrder();
	}*/
}
