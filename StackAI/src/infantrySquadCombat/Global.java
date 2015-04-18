package infantrySquadCombat;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class Global {
	
	static final int FRIENDLY_COURAGE_BONUS = 1;
	static final int WOUNDED_FRIENDLIES_COURAGE_BONUS = 2;
	static final int MAX_COVER = 10;
	static final int MAX_PROTECTION = 20;
	
	private static Condition unitUnderFire;
	private static Condition unitNotUnderFire;
	
	private static Condition lowMedRange;
	private static Condition medRange;
	private static Condition medHighRange;
	private static Condition highRange;
	
	private static Condition unitPinned;
	private static Condition unitNotPinned;
	
	private static Condition goodCoverAvailable;
	private static Condition poorCoverAvailable;
	
	private static Condition unitInCover;
	private static Condition unitNotInCover;
	
	private static Condition lowThreatLevel;
	private static Condition medThreatLevel;
	private static Condition medHighThreatLevel;
	private static Condition highThreatLevel;
	
	private static Condition highCourage;
	private static Condition lowCourage;
	
	private static Condition highPanic;
	private static Condition lowPanic;
	
	private static Condition highMorale;
	private static Condition lowMorale;
	
	private static Condition alliesWounded;
	private static Condition alliesNotWounded;
	
	private static Condition enemyCoverGood;
	private static Condition enemyCoverPoor;

	private static Condition enemySuppressed;
	private static Condition enemyNotSuppressed;
	
	private static Condition enemyScattered;
	private static Condition enemyNotScattered;

	private static Condition noCondition;
	
	
	private static Action proneFromStanding;
	private static Action proneFromCrouch;
	private static Action crouchFromStanding;
	private static Action crouchFromProne;
	private static Action standFromProne;
	private static Action standFromCrouch;
	private static Action duckDown;
	private static Action popUp;
	private static Action blindFire;
	private static Action crawlForward;
	private static Action crawlBackward;
	private static Action moveForward;
	private static Action moveBackward;
	private static Action sprintForward;
	private static Action sprintBackward;
	private static Action aimStanding;
	private static Action aimCrouched;
	private static Action aimProne;
	private static Action checkAmmo;
	private static Action reload;
	private static Action fireShot;
	private static Action findTarget;
	private static Action findCover;
	private static Action leave;
	private static Action doNothing;
	
	
	public static Response takeCover;
	public static Response prone;
	public static Response sniperFire;
	public static Response suppressingFire;
	public static Response advance;
	public static Response regroup;
	public static Response retreat;
	public static Response advancingFire;
	public static Response fallBack;	
	public static Response aimedFire;
	public static Response approach;
	public static Response withdraw;
	public static Response rout;
	public static Response surrender;
	public static Response wait;
	
	public static List<Response> allResponses = new ArrayList<>();
	public static List<Condition> allConditions = new ArrayList<>();
	
	public static void createAllResponses() {
		responseTakeCover();
		responseProne();
		responseSniperFire();
		responseSuppressingFire();
		responseAdvance();
		responseRegroup();
		responseRetreat();
		responseAdvancingFire();
		responseFallBack();
		responseAimedFire();
		responseApproach();
		responseWithdraw();
		responseRout();
		responseSurrender();
		responseWait();
	}

	private static void responseWait() {
		wait = new Response("wait", "do nothing");
		wait.addCondition(noCondition);
		wait.addAction(doNothing);
		allResponses.add(wait);
	}

	private static void responseSurrender() {
		surrender = new Response("surrender", "completely cease hostilities");
		surrender.addCondition(unitPinned);
		surrender.addCondition(lowPanic);
		surrender.addCondition(lowMorale);
	}

	private static void responseRout() {
		rout = new Response("rout", "flee from combat");
		rout.addCondition(medHighThreatLevel);
		rout.addCondition(highPanic);
	}

	private static void responseWithdraw() {
		withdraw = new Response("withdraw", "leave the battlefield");
		withdraw.addCondition(highRange);
		withdraw.addCondition(unitNotPinned);
		withdraw.addCondition(highThreatLevel);
		withdraw.addCondition(lowCourage);
		withdraw.addCondition(lowPanic);
		withdraw.addCondition(alliesNotWounded);
	}

	private static void responseApproach() {
		approach = new Response("approach", "move to within range of the enemy");
		approach.addCondition(unitNotUnderFire);
		approach.addCondition(medHighRange);
		approach.addCondition(unitNotPinned);
		approach.addCondition(lowThreatLevel);
		approach.addAction(standFromCrouch);
		approach.addAction(standFromProne);
		approach.addAction(moveForward);
		allResponses.add(approach);
	}

	private static void responseAimedFire() {
		aimedFire = new Response("direct aimed fire", "direct precision fire at targets");
		aimedFire.addCondition(lowMedRange);
		aimedFire.addCondition(unitNotPinned);
		aimedFire.addCondition(goodCoverAvailable);
		aimedFire.addCondition(unitInCover);
		aimedFire.addAction(popUp);
		aimedFire.addAction(findTarget);
		aimedFire.addAction(aimStanding);
		aimedFire.addAction(fireShot);
		aimedFire.addAction(duckDown);
		aimedFire.addAction(checkAmmo);
		allResponses.add(aimedFire);
	}

	private static void responseFallBack() {
		fallBack = new Response("retreat", "move backward while laying down covering fire");
		fallBack.addCondition(lowMedRange);
		fallBack.addCondition(unitNotPinned);
		fallBack.addCondition(medHighThreatLevel);
		fallBack.addCondition(lowPanic);
		fallBack.addCondition(enemySuppressed);
	}

	private static void responseAdvancingFire() {
		advancingFire = new Response("advancing fire", "move forward while laying down covering fire");
		advancingFire.addCondition(lowMedRange);
		advancingFire.addCondition(unitNotPinned);
		advancingFire.addCondition(lowThreatLevel);
		advancingFire.addCondition(highCourage);
		advancingFire.addCondition(lowPanic);
		advancingFire.addCondition(enemySuppressed);
	}

	private static void responseRetreat() {
		retreat = new Response("retreat", "take a quick sprint backward");
		retreat.addCondition(lowMedRange);
		retreat.addCondition(unitNotPinned);
		retreat.addCondition(poorCoverAvailable);
		retreat.addCondition(highThreatLevel);
		retreat.addAction(standFromCrouch);
		retreat.addAction(standFromProne);
		retreat.addAction(sprintBackward);
		allResponses.add(retreat);
	}

	private static void responseRegroup() {
		regroup = new Response("regroup", "move to the nearest ally");
		regroup.addCondition(unitNotPinned);
		regroup.addCondition(poorCoverAvailable);
		regroup.addCondition(lowCourage);
		regroup.addCondition(enemySuppressed);
	}

	private static void responseAdvance() {
		advance = new Response("advance", "take a quick sprint forward");
		advance.addCondition(medHighRange);
		advance.addCondition(unitNotPinned);
		advance.addCondition(medThreatLevel);
		advance.addCondition(highCourage);
		advance.addCondition(lowPanic);
		advance.addCondition(enemySuppressed);
		
		advance.addAction(standFromCrouch);
		advance.addAction(standFromProne);
		advance.addAction(sprintForward);
		allResponses.add(advance);
	}

	private static void responseSuppressingFire() {
		suppressingFire = new Response("suppressing fire", "direct high volume unaimed fire at targets");
		suppressingFire.addCondition(lowMedRange);
		suppressingFire.addCondition(enemyNotSuppressed);
		suppressingFire.addAction(popUp);
		suppressingFire.addAction(findTarget);
		suppressingFire.addAction(fireShot);
		suppressingFire.addAction(fireShot);
		suppressingFire.addAction(duckDown);
		suppressingFire.addAction(checkAmmo);
		allResponses.add(suppressingFire);
	}

	private static void responseSniperFire() {
		sniperFire = new Response("sniper fire", "direct long range precision fire at targets");
		sniperFire.addCondition(highRange);
		sniperFire.addCondition(unitNotPinned);
		sniperFire.addCondition(goodCoverAvailable);
		sniperFire.addCondition(unitInCover);
		sniperFire.addCondition(lowPanic);
	}

	private static void responseProne() {
		prone = new Response("go prone", "crawl forward");
		prone.addCondition(unitUnderFire);
		prone.addCondition(medRange);
		prone.addCondition(unitNotPinned);
		prone.addCondition(poorCoverAvailable);
		prone.addCondition(medHighThreatLevel);
		prone.addCondition(highCourage);
		prone.addCondition(lowPanic);
		prone.addAction(proneFromStanding);
		prone.addAction(proneFromCrouch);
		prone.addAction(crawlForward);
		allResponses.add(prone);
	}

	private static void responseTakeCover() {
		takeCover = new Response("take cover", "take position behind obstacles or terrain to avoid enemy fire");
		takeCover.addCondition(unitUnderFire);
		takeCover.addCondition(lowMedRange);
		takeCover.addCondition(goodCoverAvailable);
		takeCover.addCondition(unitNotInCover);
		takeCover.addAction(findCover);
		allResponses.add(takeCover);
	}

	public static void createAllActions() {
		actionProneFromStanding();
		actionProneFromCrouch();
		actionCrouchFromStanding();
		actionCrouchFromProne();
		actionStandFromProne();
		actionStandFromCrouch();
		actionDuckDown();
		actionPopUp();
		actionBlindFire();
		actionCrawlForward();
		actionMoveForward();
		actionSprintForward();
		actionSprintBackward();
		actionFindTarget();
		actionFindCover();
		actionAimStanding();
		actionCheckAmmo();
		actionReload();
		actionFireShot();
		actionDoNothing();
	}

	private static void actionDoNothing() {
		doNothing = new Action ("do nothing", "wait and do nothing", 1000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionFireShot() {
		fireShot = new Action ("fire shot", "fire a single shot at target", 1000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getEnemyTarget() == null) {
					return false;
				}
				
				parent.attackTarget();
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionReload() {
		reload = new Action ("reload", "reload your primary weapon if able", 4000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				parent.reload();
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionCheckAmmo() {
		checkAmmo = new Action ("check ammo", "check if your primary weapon needs to be reloaded", 2000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				int ammoRemaining = parent.equippedWeapon.getRoundsRemaining();
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				if(ammoRemaining == 0) {
					reload.perform(parent);
				}
				
				return true;
			}
		};
	}

	private static void actionAimStanding() {
		aimStanding = new Action ("aim standing", "aim at target while standing", 5000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getEnemyTarget() == null) {
					return false;
				}
				
				if(parent.aiming == true) {
					return false;
				}
				
				parent.aiming = true;
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionFindCover() {
		findCover = new Action ("find cover", "take cover behind nearby obstacles", 2000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.inCover != 0) {
					return false;
				}
				
				parent.inCover = 2;
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionFindTarget() {
		findTarget = new Action("find target", "choose the most threatening target", 1000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.allThreats.size() == 0) {
					return false;
				}
				
				if(parent.getEnemyTarget() != null) {
					return false;
				}
				
				Threat highestThreat = Collections.max(parent.allThreats);
				Unit target = highestThreat.getTarget();
				
				parent.chooseEnemyTarget(target);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionSprintBackward() {
		sprintBackward = new Action ("sprint forward", "run forward a short distance", 2000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 2) {
					return false;
				}
				
				int distanceMoved = -2;
				parent.movePosition(distanceMoved);

				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionSprintForward() {
		sprintForward = new Action ("sprint forward", "run forward a short distance", 2000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 2) {
					return false;
				}
				
				int distanceMoved = 2;
				parent.movePosition(distanceMoved);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionMoveForward() {
		moveForward = new Action ("move forward", "walk forward", 5000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 2) {
					return false;
				}
				
				int distanceMoved = 1;
				parent.movePosition(distanceMoved);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionCrawlForward() {
		crawlForward = new Action ("move forward", "walk forward", 10000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 1) {
					return false;
				}
				
				int distanceMoved = 1;
				parent.movePosition(distanceMoved);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionBlindFire() {
		blindFire = new Action ("blind fire", "fire while staying down behind cover", 1000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.inCover != 2) {
					return false;
				}
				
				if(parent.getEnemyTarget() == null) {
					return false;
				}
				
				parent.attackTarget();
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionPopUp() {
		popUp = new Action ("pop up", "pop up to fire", 1000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.inCover != 2) {
					return false;
				}
				
				parent.inCover = 1;
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionDuckDown() {
		duckDown = new Action ("duck down", "duck down behind cover", 1000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.inCover != 1) {
					return false;
				}
				
				parent.inCover = 2;
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionStandFromCrouch() {
		standFromCrouch = new Action ("stand from crouch", "stand up from a kneeling position", 1000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 1) {
					return false;
				}
				
				parent.changeStance(2);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionStandFromProne() {
		standFromProne = new Action ("stand from prone", "stand up from a prone position", 2000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 0) {
					return false;
				}
				
				parent.changeStance(2);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionCrouchFromProne() {
		crouchFromProne = new Action ("crouch from prone", "get onto your knees from your belly", 1500) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 0) {
					return false;
				}
				
				parent.changeStance(1);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionCrouchFromStanding() {
		crouchFromStanding = new Action ("crouch from standing", "kneel down", 1000) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 2) {
					return false;
				}
				
				parent.changeStance(1);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionProneFromCrouch() {
		proneFromCrouch = new Action ("go prone from crouch", "get down onto your belly from a kneeling position", 1200) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 1) {
					return false;
				}
				
				parent.changeStance(0);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	private static void actionProneFromStanding() {
		proneFromStanding = new Action ("go prone from standing", "get down onto your belly from a standing position", 1500) {
			@Override
			public boolean perform(Unit parent) throws InterruptedException {
				if(parent.getStance() != 2) {
					return false;
				}
				
				parent.changeStance(0);
				
				int sleepTime = getTimeToComplete();
				Thread.sleep(sleepTime);
				
				return true;
			}
		};
	}

	public static void createAllConditions() {
		conditionUnitUnderFire();
		conditionUnitNotUnderFire();
		conditionLowMedRange();
		conditionMedRange();
		conditionMedHighRange();
		conditionHighRange();
		conditionUnitPinned();
		conditionUnitNotPinned();
		conditionGoodCoverAvailable();
		conditionPoorCoverAvailable();
		conditionUnitInCover();
		conditionUnitNotInCover();
		conditionLowThreatLevel();
		conditionMedThreatLevel();
		conditionMedHighThreatLevel();
		conditionHighThreatLevel();
		conditionHighCourage();
		conditionLowCourage();
		conditionHighPanic();
		conditionLowPanic();
		conditionHighMorale();
		conditionLowMorale();
		conditionAlliesWounded();
		conditionAlliesNotWounded();
		conditionEnemyCoverGood();
		conditionEnemyCoverPoor();
		conditionEnemySuppressed();
		conditionEnemyNotSuppressed();
		conditionEnemyScattered();
		conditionEnemyNotScattered();
		conditionNoCondition();
	}
	
	public static List<Condition> getListOfAllConditionsFromCode (int conditionCode) {
		List<Condition> listOfConditions = new ArrayList<>();
		int conditionCodeRemaining = conditionCode;
		
		while(conditionCodeRemaining > 0) {
			int conditionIdToFind = (int) (Math.log(conditionCodeRemaining) / Math.log(2.0));
			conditionCodeRemaining -= Math.pow(conditionIdToFind, 2.0);
			
			Condition looksLike = new Condition(conditionIdToFind);
			int index = allConditions.indexOf(looksLike);
			Condition conditionToAdd = allConditions.get(index);
			listOfConditions.add(conditionToAdd);
		}
		
		return listOfConditions;
	}

	private static void conditionNoCondition() {
		noCondition = new Condition();
	}

	private static void conditionEnemyNotScattered() {
		enemyNotScattered = new Condition() {
			public boolean test(Unit parent) {
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				Squad enemySquad = enemyThreat.getSquad();
				return ! enemySquad.squadScattered;
			}
		};
		allConditions.add(enemyNotScattered);
	}

	private static void conditionEnemyScattered() {
		enemyScattered = new Condition() {
			public boolean test(Unit parent) {
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				Squad enemySquad = enemyThreat.getSquad();
				return enemySquad.squadScattered;
			}
		};
		allConditions.add(enemyScattered);
	}

	private static void conditionEnemyNotSuppressed() {
		enemyNotSuppressed = new Condition() {
			public boolean test(Unit parent) {
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				return ! enemyThreat.suppressed;
			}
		};
		allConditions.add(enemyNotSuppressed);
	}

	private static void conditionEnemySuppressed() {
		enemySuppressed = new Condition() {
			public boolean test(Unit parent) {
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				return enemyThreat.suppressed;
			}
		};
		allConditions.add(enemySuppressed);
	}

	private static void conditionEnemyCoverPoor() {
		enemyCoverPoor = new Condition() {
			public boolean test(Unit parent) {
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				int enemyCover = enemyThreat.cover;
				if(enemyCover > 4) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(enemyCoverPoor);
	}

	private static void conditionEnemyCoverGood() {
		enemyCoverGood = new Condition() {
			public boolean test(Unit parent) {
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				int enemyCover = enemyThreat.cover;
				if(enemyCover < 5) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(enemyCoverGood);
	}

	private static void conditionAlliesNotWounded() {
		alliesNotWounded = new Condition() {
			public boolean test(Unit parent) {
				Squad squadMembers = parent.getSquad();
				return ! squadMembers.getUnitsWounded();
			}
		};
		allConditions.add(alliesNotWounded);
	}

	private static void conditionAlliesWounded() {
		alliesWounded = new Condition() {
			public boolean test(Unit parent) {
				Squad squadMembers = parent.getSquad();
				return squadMembers.getUnitsWounded();
			}
		};
		allConditions.add(alliesWounded);
	}

	private static void conditionLowMorale() {
		lowMorale = new Condition() {
			public boolean test(Unit parent) {
				Squad squadMembers = parent.getSquad();
				if(squadMembers.getMorale() > 5) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(lowMorale);
	}

	private static void conditionHighMorale() {
		highMorale = new Condition() {
			public boolean test(Unit parent) {
				Squad parentSquad = parent.getSquad();
				if(parentSquad.getMorale() < 6) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(highMorale);
	}

	private static void conditionLowPanic() {
		lowPanic = new Condition() {
			public boolean test(Unit parent) {
				if(parent.panic > 5) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(lowPanic);
	}

	private static void conditionHighPanic() {
		highPanic = new Condition() {
			public boolean test(Unit parent) {
				if(parent.panic < 6) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(highPanic);
	}

	private static void conditionLowCourage() {
		lowCourage = new Condition() {
			public boolean test(Unit parent) {
				if(parent.courage > 5) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(lowCourage);
	}

	private static void conditionHighCourage() {
		highCourage = new Condition() {
			public boolean test(Unit parent) {
				if(parent.courage < 6) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(highCourage);
	}

	private static void conditionHighThreatLevel() {
		highThreatLevel = new Condition() {
			public boolean test(Unit parent) {
				if(parent.threatLevel < 7) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(highThreatLevel);
	}

	private static void conditionMedHighThreatLevel() {
		medHighThreatLevel = new Condition() {
			public boolean test(Unit parent) {
				if(parent.threatLevel < 4) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(medHighThreatLevel);
	}

	private static void conditionMedThreatLevel() {
		medThreatLevel = new Condition() {
			public boolean test(Unit parent) {
				if(parent.threatLevel < 4) {
					return false;
				}
				if(parent.threatLevel > 6) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(medThreatLevel);
	}

	private static void conditionLowThreatLevel() {
		lowThreatLevel = new Condition() {
			public boolean test(Unit parent) {
				if(parent.threatLevel > 3) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(lowThreatLevel);
	}

	private static void conditionUnitNotInCover() {
		unitNotInCover = new Condition() {
			public boolean test(Unit parent) {
				if(parent.inCover != 0) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(unitNotInCover);
	}

	private static void conditionUnitInCover() {
		unitInCover = new Condition() {
			public boolean test(Unit parent) {
				if(parent.inCover == 0) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(unitInCover);
	}

	private static void conditionPoorCoverAvailable() {
		poorCoverAvailable = new Condition() {
			public boolean test(Unit parent) {
				if(parent.cover > 4) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(poorCoverAvailable);
	}

	private static void conditionGoodCoverAvailable() {
		goodCoverAvailable = new Condition() {
			public boolean test(Unit parent) {
				if(parent.cover < 5) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(goodCoverAvailable);
	}

	private static void conditionUnitNotPinned() {
		unitNotPinned = new Condition() {
			public boolean test(Unit parent) {
				return ! parent.suppressed;
			}
		};
		allConditions.add(unitNotPinned);
	}

	private static void conditionUnitPinned() {
		unitPinned = new Condition() {
			public boolean test(Unit parent) {
				return parent.suppressed;
			}
		};
		allConditions.add(unitPinned);
	}

	private static void conditionHighRange() {
		highRange = new Condition() {
			public boolean test(Unit parent) {
				int parentPosition = parent.getPosition();
				
				if(parent.allThreats.size() == 0) {
					return false;
				}
				
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				int enemyPosition = enemyThreat.getPosition();
				
				int range = Math.abs(parentPosition - enemyPosition) + 1;
				if(range < 7) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(highRange);
	}

	private static void conditionMedHighRange() {
		medHighRange = new Condition() {
			public boolean test(Unit parent) {
				int parentPosition = parent.getPosition();
				
				if(parent.allThreats.size() == 0) {
					return false;
				}
				
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				int enemyPosition = enemyThreat.getPosition();
				
				int range = Math.abs(parentPosition - enemyPosition) + 1;
				if(range < 4) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(medHighRange);
	}

	private static void conditionMedRange() {
		medRange = new Condition() {
			public boolean test(Unit parent) {
				int parentPosition = parent.getPosition();
				
				if(parent.allThreats.size() == 0) {
					return false;
				}
				
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				int enemyPosition = enemyThreat.getPosition();
				
				int range = Math.abs(parentPosition - enemyPosition) + 1;
				if(range < 4) {
					return false;
				}
				if(range > 6) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(medRange);
	}

	private static void conditionLowMedRange() {
		lowMedRange = new Condition() {
			public boolean test(Unit parent) {
				int parentPosition = parent.getPosition();
				
				if(parent.allThreats.size() == 0) {
					return false;
				}
				
				Threat greatestThreat = Collections.max(parent.allThreats);
				Unit enemyThreat = greatestThreat.getTarget();
				int enemyPosition = enemyThreat.getPosition();
				
				int range = Math.abs(parentPosition - enemyPosition) + 1;
				if(range > 6) {
					return false;
				}
				return true;
			}
		};
		allConditions.add(lowMedRange);
	}

	private static void conditionUnitNotUnderFire() {
		unitNotUnderFire = new Condition() {
			public boolean test(Unit parent) {
				return ! parent.underFire;
			}
		};
		allConditions.add(unitNotUnderFire);
	}

	private static void conditionUnitUnderFire() {
		unitUnderFire = new Condition() {
			public boolean test(Unit parent) {
				return parent.underFire;
			}
		};
		allConditions.add(unitUnderFire);
	}
}
