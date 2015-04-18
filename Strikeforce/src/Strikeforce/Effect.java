package Strikeforce;

import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.List;

import static Strikeforce.Global.*;

public class Effect extends Entity {
	
	private List<BufferedImage> animationImages = new ArrayList<>();
	private int frameSpeed;
	private int count = 0;
	private boolean animationOver = false;
	protected int damage;
	protected boolean hitsAir;
	protected boolean hitsGround;
	
	public Effect(String inName, int inX, int inY, int inDirection, int inAltitude, int frames, int inFrameSpeed) {
		super(inName + "1", inX, inY, inDirection, inAltitude);
		for(int i = 2; i <= frames; i++) {
			animationImages.add(loadImage(inName + i + ".png"));
		}
		frameSpeed = inFrameSpeed;
		hitsAir = false;
		hitsGround = false;
	}
	
	public Effect(String inName, int inX, int inY, int inDirection, int inAltitude, int frames, int inFrameSpeed, 
			int inDamage, boolean inHitsAir, boolean inHitsGround) {
		super(inName + "1", inX, inY, inDirection, inAltitude);
		for(int i = 2; i <= frames; i++) {
			animationImages.add(loadImage(inName + i + ".png"));
		}
		frameSpeed = inFrameSpeed;
		damage = inDamage;
		hitsAir = inHitsAir;
		hitsGround = inHitsGround;
	}
	
	public boolean getAnimationOver() {
		return animationOver;
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

	public void animate() {
		if(animationOver == true) {
			return;
		}
		
		int frame = count / frameSpeed;
		
		currentImage = animationImages.get(frame);
		count++;
		
		if(frame == (animationImages.size() - 1)) {
			animationOver = true;
		}
	}
}
