package Engine;
import static org.junit.Assert.*;

import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.junit.Before;
import org.junit.Test;

import com.sun.media.jfxmedia.AudioClip;

import static Engine.Global.*;

public class AnimationTest {
	
	Animation animation;
	Animation single;

	@Before
	public void setUp() throws Exception {
		Map<Integer, MediaClip> clips = new HashMap<>(5, 1f);
		
		animation = new Animation("Test");
		
		MediaClip clip = new MediaClip("sound", "", "", "wav");
		clips.put(1, clip);

		for(int i = 1; i <= 3; i++) {
			BufferedImage imageToAdd = loadImage("test.png");
			MediaClip clipToAdd = clips.get(i);
			
			animation.addFrame(imageToAdd, clipToAdd);
		}
		
		single = new Animation("Single");
		BufferedImage imageToAdd = loadImage("test.png");
		single.addFrame(imageToAdd, null);
	}

	@Test
	public void playAnimation() {
		BufferedImage image = animation.getImage();
		MediaClip clip = animation.getClip();
		boolean hasNext = animation.hasNext();
		
		assertTrue(image != null);
		assertTrue(clip != null);
		assertTrue(hasNext == true);
		
		animation.next();
		
		image = animation.getImage();
		clip = animation.getClip();
		hasNext = animation.hasNext();
		
		assertTrue(image != null);
		assertTrue(clip == null);
		assertTrue(hasNext == true);
		
		animation.next();
		
		image = animation.getImage();
		clip = animation.getClip();
		hasNext = animation.hasNext();
		
		assertTrue(image != null);
		assertTrue(clip == null);
		assertTrue(hasNext == false);
		
		animation.next();
		
		image = animation.getImage();
		clip = animation.getClip();
		hasNext = animation.hasNext();
		
		assertTrue(image != null);
		assertTrue(clip != null);
		assertTrue(hasNext == true);
	}
	
	@Test
	public void playSingle() {
		BufferedImage image = single.getImage();
		MediaClip clip = single.getClip();
		boolean hasNext = single.hasNext();
		
		assertTrue(image != null);
		assertTrue(clip == null);
		assertTrue(hasNext == false);
		
		single.next();
		
		image = single.getImage();
		clip = single.getClip();
		hasNext = single.hasNext();
		
		assertTrue(image != null);
		assertTrue(clip == null);
		assertTrue(hasNext == false);
	}
}
