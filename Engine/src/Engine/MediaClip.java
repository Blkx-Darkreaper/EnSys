package Engine;

import java.io.IOException;
import javax.sound.sampled.AudioFileFormat;
import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.Clip;
import javax.sound.sampled.LineEvent;
import javax.sound.sampled.LineListener;
import javax.sound.sampled.LineUnavailableException;
import javax.sound.sampled.UnsupportedAudioFileException;
import static Engine.Global.*;

public class MediaClip {

	protected String name;
	protected String path;
	protected String addon;
	protected String extension;
	protected boolean loop = false;
	protected AudioInputStream audioStream;
	protected Clip clip;
	protected LineListener listener;
	
	public MediaClip(String inName, String inPath, String inAddon, String inExtension) {
		this(inName, inPath, inAddon, inExtension, false);
	}
	
	public MediaClip(String inName, String inPath, String inAddon, String inExtension, boolean doesLoop) {
		name = inName;
		path = inPath;
		addon = inAddon;
		extension = inExtension;
		loop = doesLoop;
		
		String filename = getFilename(path, name, addon, extension);
		audioStream = loadAudio(filename);
		
		listener = new LineListener() {
			
			@Override
			public void update(LineEvent event) {
				if (event.getType() != LineEvent.Type.STOP) {
			        return;
			    }
				
				if(clip == null) {
					return;
				}
				
				clip.close();
				clip = null;
			}
		};
	}
	
	public void getFormat() {
		try {
			AudioFileFormat format = AudioSystem.getAudioFileFormat(audioStream);
			System.out.println("Format: " + format.toString());
		} catch (UnsupportedAudioFileException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void play() {
		if(clip == null) {
			try {
				clip = AudioSystem.getClip();
			} catch (LineUnavailableException e) {
				e.printStackTrace();
				return;
			}
		}
		
		boolean playing = clip.isRunning();
		if(playing == true) {
			return;
		}
		
		boolean lineOpen = clip.isOpen();
		if(lineOpen == false) {
			openLine();
		}
		
		clip.setFramePosition(0);
		clip.start();
	}

	private void openLine() {
		try {
			clip.open(audioStream);
		} catch (LineUnavailableException e) {
			e.printStackTrace();
			return;
		} catch (IOException e) {
			e.printStackTrace();
			return;
		}
		
		if(loop == true) {
			clip.setLoopPoints(0, -1);
			clip.loop(Clip.LOOP_CONTINUOUSLY);
		}
		
		//clip.addLineListener(listener);
	}
}