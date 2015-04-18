package Strikeforce;

import static Strikeforce.Global.resLoader;

import java.util.ArrayList;
import java.util.List;

import javax.swing.ImageIcon;

public abstract class Weapon {
	
	private String name;
	private String description;
	
	protected int muzzleVelocity;
	protected int damage;
	protected int cooldown;
	protected int reload = 0;
	
	public Weapon (String inName, String inDescription, int inMuzzleVelocity, int inDamage, int inCooldown) {
		name = inName;
		description = inDescription;
		muzzleVelocity = inMuzzleVelocity;
		damage = inDamage;
		cooldown = inCooldown;
	}
	
	public String getName() {
		return name;
	}
	
	public String getDescription() {
		return description;
	}

	public List<Projectile> openFire(int originX, int originY, int inDirection, int inAltitude) {
		if(reload > 0) {
			return null;
		}
		
		List<Projectile> allShots = new ArrayList<>();
		
		String bulletName = "bullet";
		ImageIcon bulletIcon = resLoader.getImageIcon(bulletName + ".png");
		int startX = originX;
		int startY = originY + bulletIcon.getIconHeight() / 2;
		boolean hitsAir = true;
		boolean hitsGround = false;
		boolean live = true;
		
		/*allShots.add(new Projectile(bulletIcon, startX, startY, muzzleVelocity, 
				damage, inDirection, inAltitude));*/
		allShots.add(new Projectile(bulletName, startX, startY, inDirection, inAltitude, 
				muzzleVelocity, damage, hitsAir, hitsGround, live));
		
		reload += cooldown;
		
		return allShots;
	}
	
	public void reload() {
		if(reload == 0) {
			return;
		}
		
		reload--;
	}
}
