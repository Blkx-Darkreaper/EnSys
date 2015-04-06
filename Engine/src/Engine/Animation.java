package Engine;

import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import javax.sound.sampled.Clip;

import com.sun.media.jfxmedia.AudioClip;

public class Animation {

	protected String name;
	protected int count = 1;
	protected int lastFrame;
	protected Map<Integer, Keyframe> allKeyframes;
	protected BufferedImage currentImage;
	
	public Animation(String inName) {
		name = inName;
		lastFrame = 0;
		allKeyframes = new HashMap<>();
	}
	
	public String getName() {
		return name;
	}
	
	public int getLength() {
		return lastFrame;
	}
	
	public BufferedImage getImage() {
		checkForKeyframes();
		
		Keyframe toCheck = allKeyframes.get(count);
		if(toCheck == null) {
			return currentImage;
		}
		
		BufferedImage toGet = toCheck.getImage();
		currentImage = toGet;
		return toGet;
	}

	public MediaClip getClip() {
		checkForKeyframes();
		
		Keyframe toCheck = allKeyframes.get(count);
		if(toCheck == null) {
			return null;
		}
		
		MediaClip toGet = toCheck.getClip();
		return toGet;
	}
	
	public void addFrame() {
		lastFrame++;
	}
	
	public void addFrame(BufferedImage imageToAdd, MediaClip clipToAdd) {
		if(imageToAdd == null && clipToAdd == null) {
			addFrame();
			return;
		}
		
		addKeyframe(imageToAdd, clipToAdd);
	}
	
	public void addKeyframe(BufferedImage inImage, MediaClip inClip) {
		if(inImage == null && inClip == null) {
			throw new IllegalArgumentException("Keyframe needs a non-null image or MediaClip");
		}
		
		lastFrame++;
		Keyframe toAdd = new Keyframe(lastFrame, inImage, inClip);
		allKeyframes.put(lastFrame, toAdd);
		
		if(currentImage == null) {
			currentImage = inImage;
		}
	}
	
	private void checkForKeyframes() {
		int keyframesAdded = allKeyframes.size();
		if(keyframesAdded != 0) {
			return;
		}
		
		throw new IllegalStateException("No Keyframes added to animation");
	}
	
	public void next() {
		checkForKeyframes();
		
		if(hasNext() == false) {
			count = 1;
			return;
		}
		
		count++;
	}
	
	public void previous() {
		checkForKeyframes();
		
		if(hasPrevious() == false) {
			count = lastFrame;
			return;
		}
		
		count--;
	}
	
	public boolean hasNext() {
		checkForKeyframes();
		
		if(count >= lastFrame) {
			return false;
		}
			
		return true;
	}
	
	public boolean hasPrevious() {
		checkForKeyframes();
		
		if(count <= 1) {
			return false;
		}
		
		return true;
	}
	
	protected class Keyframe {
		
		protected int frameIndex;
		protected BufferedImage image;
		protected MediaClip clip;
		
		public Keyframe(int inIndex, BufferedImage inImage, MediaClip inClip) {
			frameIndex = inIndex;
			image = inImage;
			clip = inClip;
		}
		
		public Keyframe(int inIndex, BufferedImage inImage) {
			this(inIndex, inImage, null);
		}
		
		public Keyframe(int inIndex, MediaClip inClip) {
			this(inIndex, null, inClip);
		}
		
		public int hashCode() {
			return frameIndex;
		}
		
		public BufferedImage getImage() {
			return image;
		}
		
		public MediaClip getClip() {
			return clip;
		}
	}
}
