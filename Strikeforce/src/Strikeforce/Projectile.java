package Strikeforce;

import javax.swing.ImageIcon;

import static Strikeforce.Board.*;

public class Projectile extends Mover {
	
	protected int damage;
	protected boolean hitsAir;
	protected boolean hitsGround;
	protected boolean live;
	protected boolean detonate = false;
	
	public Projectile(String inName, int inX, int inY, int inDirection, int inAltitude, int inSpeed, int inDamage, 
			boolean inHitsAir, boolean inHitsGround, boolean inLive) {
		super(inName, inX, inY, inDirection, inAltitude, inSpeed);
		damage = inDamage;
		hitsAir = inHitsAir;
		hitsGround = inHitsGround;
		live = inLive;
	}
	
	public int getDamage() {
		return damage;
	}
	
	public boolean getHitsAir() {
		return hitsAir;
	}
	
	public boolean getHitsGround() {
		return hitsGround;
	}
	
	public boolean getLive() {
		return live;
	}
	
	public boolean getDetonate() {
		return detonate;
	}
	
	public Effect getExplosionAnimation() {
		return null;
	}
}
