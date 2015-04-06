package Engine;
import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Font;
import java.awt.Graphics2D;
import java.awt.Point;
import java.awt.Rectangle;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.Random;
import java.util.logging.FileHandler;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;
import javax.imageio.ImageIO;
import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.UnsupportedAudioFileException;
import javax.swing.JComponent;

public class Global {

	public static final int DEFAULT_TIME_INTERVAL = 20;
	public static final Font DEFAULT_FONT = new Font("GenericSansSerif", Font.PLAIN, 10);
	public static Random random;
	public static Logger clientLogger;
	public static Logger serverLogger;
	public static FileHandler clientFileHandler;
	public static FileHandler serverFileHandler;
	public static String root = System.getProperty("user.dir");
	
	public static String getFilename(String path, String name, String addon, String extension) {
		if(path == null) {
			path = "";
		}
		if(addon == null) {
			addon = "";
		}
		String filename = path + name + addon + "." + extension;
		return filename;
	}
	
	public static BufferedImage loadImage(String filename) {
		BufferedImage image;
		try {
			File imageFile = new File(root + "/images/" + filename);
			image = ImageIO.read(imageFile);
		} catch (IOException e) {
			System.out.println("Image could not be read");
			image = null;
		}
		return image;
	}
	
	public static AudioInputStream loadAudio(String filename) {
		AudioInputStream audioStream;
		try {
			File mediaFile = new File(root + "/audio/" + filename);
			audioStream = AudioSystem.getAudioInputStream(mediaFile);
		} catch (IOException e) {
			System.out.println("Audio file could not be read");
			audioStream = null;
		} catch (UnsupportedAudioFileException e) {
			System.out.println("Audio file format not supported");
			audioStream = null;
		}
		return audioStream;
	}
	
	public static void drawImage(Graphics2D g2d, BufferedImage image, int absCornerX,
			int absCornerY, int scaledWidth, int scaledHeight) {
		// g2d.drawImage(image, absolutePositionX, absolutePositionY, null);
		g2d.drawImage(image, absCornerX, absCornerY, scaledWidth,
				scaledHeight, null);
	}
	
	public static int encodeChoices(List<Integer> selectedOptions) {
		int codedOptions = 0;

		for(Integer index : selectedOptions) {
			//codedOptions += Math.pow(2, index);
			
			int codedOption = 1 << index;
			codedOptions = codedOptions | codedOption;
		}
		
		return codedOptions;
	}
	
	public static <E> List<E> decodeChoices(int codedOptions, List<E> allOptions) {
		List<E> selectedOptions = new ArrayList<>();
		
/*		while(codedOptions > 0) {
			int index = (int) (Math.log10(codedOptions)/Math.log10(2));
			
			E selectedOption = allOptions.get(index);
			selectedOptions.add(selectedOption);
			
			codedOptions -= Math.pow(2, index);
		}*/
		int index = 0;
		while(codedOptions > 0) {
			int lsb = codedOptions & 1;
			codedOptions = codedOptions >> 1;
		
			if(lsb < 1) {
				index++;
				continue;
			}
			
			E selectedOption = allOptions.get(index);
			selectedOptions.add(selectedOption);
			index++;
		}
		
		return selectedOptions;
	}
	
	public static void initLogging(Logger logger, FileHandler handler, String logPath) {
		try {
			handler = new FileHandler(logPath);
			logger.addHandler(handler);
			SimpleFormatter formatter = new SimpleFormatter();  
	        handler.setFormatter(formatter);
		} catch (SecurityException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	public static void log(String entry, Logger logger) {
		logger.info(entry);
	}

	public static void bindAllKeys(Character character) {
		boolean pressed = true;
		character.addKeyBinding('a', pressed, "startMovingLeft");
		character.addKeyBinding('s', pressed, "startMovingDown");
		character.addKeyBinding('d', pressed, "startMovingRight");
		character.addKeyBinding('w', pressed, "startMovingUp");
		pressed = false;
		character.addKeyBinding('a', pressed, "stopMovingLeft");
		character.addKeyBinding('s', pressed, "stopMovingDown");
		character.addKeyBinding('d', pressed, "stopMovingRight");
		character.addKeyBinding('w', pressed, "stopMovingUp");
	}

	public static class Outline implements Iterator<Point>{
		protected List<Point> allPoints = new ArrayList<>();
		protected Point center;
		protected int width;
		protected int height;
		protected int nextIndex = 0;
		protected boolean hasNext = false;
		protected int shapeType;
		public static final int OVAL = 0;
		public static final int TRIANGLE = 1;
		public static final int WEDGE = 2;
		public static final int RECTANGLE = 3;
		public static final int DIAMOND = 4;
		
		public Outline(List<Point> pointsToAdd) {
			allPoints.addAll(pointsToAdd);
			
			if(allPoints.size() > 2) {
				hasNext = true;
			}
		}
		
		public Outline(int centerX, int centerY, int inWidth, int inHeight, int shapeApprox) {
			int x, y;
			Point point;
			
			switch (shapeApprox) {
			default:
			case OVAL:
				shapeType = shapeApprox;
				x = centerX - inWidth / 2;
				y = centerY + inHeight / 2;
				center = new Point(x, y);
				width = inWidth;
				height = inHeight;
				break;
				
			case TRIANGLE:
				shapeType = shapeApprox;
				
				x = centerX;
				y = centerY + inHeight/2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX + inWidth / 2;
				y = centerY - inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX - inWidth / 2;
				y = centerY - inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				break;
				
			case WEDGE:
				shapeType = shapeApprox;
				
				x = centerX - inWidth / 2;
				y = centerY + inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX + inWidth / 2;
				y = centerY + inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX;
				y = centerY - inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				break;
				
			case RECTANGLE:
				shapeType = shapeApprox;
				
				x = centerX - inWidth / 2;
				y = centerY + inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX + inWidth / 2;
				y = centerY + inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX + inWidth / 2;
				y = centerY - inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX - inWidth / 2;
				y = centerY - inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				break;
				
			case DIAMOND:
				shapeType = shapeApprox;
				
				x = centerX;
				y = centerY + inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX + inWidth / 2;
				y = centerY;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX;
				y = centerY - inHeight / 2;
				point = new Point(x, y);
				allPoints.add(point);
				
				x = centerX - inWidth / 2;
				y = centerY;
				point = new Point(x, y);
				allPoints.add(point);
				
				break;
			}
		}
		
		public Outline(int centerX, int centerY, BufferedImage image, int shapeApprox) {
			this(centerX, centerY, image.getWidth(), image.getHeight(), shapeApprox);
		}
		
		public boolean hasNext() {
			return hasNext;
		}

		public Point next() {
			if(hasNext == false) {
				if(allPoints.size() <= 2) {
 					throw new NoSuchElementException();
				}
				
				nextIndex = 0;
				hasNext = true;
			}
			
			Point nextPoint;
			
			if(nextIndex >= allPoints.size()) {
				hasNext = false;
				nextPoint = allPoints.get(0);
				
				return nextPoint;
			}
			
			nextPoint = allPoints.get(nextIndex);
			nextIndex++;
			
			return nextPoint;
		}

		public void remove() {
			throw new UnsupportedOperationException();
		}
		
		public void drawOutline(Graphics2D g2d, int viewCenterX, int viewCenterY, int viewWidth, int viewHeight) {
			if(shapeType == OVAL) {
				int centerRelativeToViewX = viewWidth / 2 + (center.x - viewCenterX);
				int centerRelativeToViewY = viewHeight / 2 - (center.y - viewCenterY);
				g2d.drawOval(centerRelativeToViewX, centerRelativeToViewY, width, height);
				return;
			}
			
			drawPolygon(g2d, viewCenterX, viewCenterY, viewWidth, viewHeight);
		}

		private void drawPolygon(Graphics2D g2d, int viewCenterX,
				int viewCenterY, int viewWidth, int viewHeight) {
			Point startPoint = next();
			Point endPoint = next();
			
			int startX, startY;
			int endX, endY;
			
			while(hasNext == true) {
				startX = viewWidth / 2 + (startPoint.x - viewCenterX);
				startY = viewHeight / 2 - (startPoint.y - viewCenterY);
				endX = viewWidth / 2 + (endPoint.x - viewCenterX);
				endY = viewHeight / 2 - (endPoint.y - viewCenterY);
				
				g2d.drawLine(startX, startY, endX, endY);
				startPoint = endPoint;
				endPoint = next();
			}
			
			startX = viewWidth / 2 + (startPoint.x - viewCenterX);
			startY = viewHeight / 2 - (startPoint.y - viewCenterY);
			endX = viewWidth / 2 + (endPoint.x - viewCenterX);
			endY = viewHeight / 2 - (endPoint.y - viewCenterY);
			g2d.drawLine(startX, startY, endX, endY);
		}
	}

	public static class KeyStrokeListener implements KeyListener {

		Character parent;
		
		public KeyStrokeListener(Character inParent) {
			parent = inParent;
		}
		
		@Override
		public void keyPressed(KeyEvent e) {
			int key = e.getKeyCode();
			int eventType = KeyEvent.KEY_PRESSED;
			
			Method toPerform = parent.getKeyAction(key, eventType);
			if(toPerform == null) {
				return;
			}
			
			//Behaviour child = parent.getBehaviour();
			try {
				toPerform.invoke(parent, null);
			} catch (IllegalAccessException e1) {
				e1.printStackTrace();
			} catch (IllegalArgumentException e1) {
				e1.printStackTrace();
			} catch (InvocationTargetException e1) {
				e1.printStackTrace();
			}
		}

		@Override
		public void keyReleased(KeyEvent e) {
			int key = e.getKeyCode();
			int eventType = KeyEvent.KEY_RELEASED;
			
			Method toPerform = parent.getKeyAction(key, eventType);
			if(toPerform == null) {
				return;
			}

			try {
				toPerform.invoke(parent, null);
			} catch (IllegalAccessException e1) {
				e1.printStackTrace();
			} catch (IllegalArgumentException e1) {
				e1.printStackTrace();
			} catch (InvocationTargetException e1) {
				e1.printStackTrace();
			}
		}

		@Override
		public void keyTyped(KeyEvent arg0) {}
	}
	
    public static class MouseDragListener extends MouseAdapter {
        protected Point lastPoint;
        protected JComponent parent;
        protected JComponent selected;
        protected boolean active = false;
        protected boolean disabled = false;
        
        public void setDisabled(boolean condition) {
        	disabled = condition;
        }
        
        public void setParent(JComponent inParent) {
        	parent = inParent;
        }

        public MouseDragListener(JComponent inParent) {
        	super();
        	
        	parent = inParent;
        }
        
        @Override
        public void mousePressed(MouseEvent event) {
        	if(parent == null) {
        		return;
        	}
        	if(disabled == true) {
        		return;
        	}
        	
        	Point click = event.getPoint();
            boolean inBounds = parent.getBounds().contains(click);
            if(inBounds == false) {
            	active = false;
            	return;
            }
            
            lastPoint = click;
            active = true;
            selected = parent;
            //selected = (JComponent) parent.getComponentAt(lastPoint);
/*            for (Component child : parent.getComponents()) {
            	boolean inBounds = child.getBounds().contains(lastPoint);
                if (inBounds == false) {
                    continue;
                }
                selected = (JComponent) child;
            }*/
            if(selected == null) {
            	lastPoint = null;
            	return;
            }
            
/*            boolean isSelectable = selected.getIsSelectable();
            if(isSelectable == false) {
            	lastPoint = null;
            	selected = null;
            	return;
            }*/
        }

        @Override
        public void mouseReleased(MouseEvent event) {
        	if(disabled == true) {
        		return;
        	}
        	if(active == false) {
        		return;
        	}
            lastPoint = null;
            selected = null;
        }

        @Override
        public void mouseDragged(MouseEvent event) {
        	if(parent == null) {
        		return;
        	}
        	if(disabled == true) {
        		return;
        	}
        	if(active == false) {
        		return;
        	}
        	if(selected == null) {
        		return;
        	}
            
            int eventX = event.getX();
            int eventY = event.getY();
            
            int dx = eventX - lastPoint.x;
            int dy = eventY - lastPoint.y;
            
        	//Point currentPosition = listeningParent.getLocation();
        	Point currentPosition = selected.getLocation();
        	
        	int currentX = currentPosition.x;
        	int currentY = currentPosition.y;
        	
            currentX += dx;
            currentY += dy;
        	
        	//listeningParent.setLocation(currentX, currentY);
        	selected.setLocation(currentX, currentY);
        	
            lastPoint = event.getPoint();
        }
    }
    
    public static class MouseSelectionListener extends MouseAdapter {
    	protected Point startPoint;
    	protected Point endPoint;
    	protected View parent;
    	protected Rectangle absSelectionBox = null;
    	protected List<Entity> selectedEntities = null;
        protected boolean selectionActive = false;
        protected boolean listenerDisabled = false;
        
        public void setDisabled(boolean condition) {
        	listenerDisabled = condition;
        }
        
        public void setParent(View inParent) {
        	parent = inParent;
        }
    	
    	public MouseSelectionListener(View inParent) {
    		super();
    		parent = inParent;
		}
    	
    	@Override
    	public void mousePressed(MouseEvent event) {
        	if(parent == null) {
        		return;
        	}
        	if(listenerDisabled == true) {
        		return;
        	}
        	
        	Point click = event.getPoint();
            boolean clickInBounds = parent.getBounds().contains(click);
            if(clickInBounds == false) {
            	selectionActive = false;
            	return;
            }
            
            selectionActive = true;
            
        	startPoint = event.getPoint();
            
            if(absSelectionBox == null) {
            	return;
            }
            
            boolean clickInCurrentSelection = absSelectionBox.contains(click);
            if(clickInCurrentSelection == false) {
            	parent.deselect(selectedEntities);
            	absSelectionBox = null;
            	parent.setSelectionBox(absSelectionBox);
            }
    		//endPoint = event.getPoint();
    		//Rectangle selectionBox = getSelectionBox(startPoint, endPoint);
    		//parent.setSelectionBox(selectionBox);
    	}

    	@Override
    	public void mouseReleased(MouseEvent event) {
        	if(listenerDisabled == true) {
        		return;
        	}
        	if(selectionActive == false) {
        		return;
        	}
        	
    		Point currentPoint = event.getPoint();

    		absSelectionBox = getSelectionBox(startPoint, currentPoint);
    		parent.setSelectionBox(absSelectionBox);
    		parent.deselect(selectedEntities);
    		selectedEntities = parent.getSelectedEntities();
    		
    		startPoint = null;
    		endPoint = null;
    	}

    	@Override
    	public void mouseDragged(MouseEvent event) {
        	if(parent == null) {
        		return;
        	}
        	if(listenerDisabled == true) {
        		return;
        	}
        	if(selectionActive == false) {
        		return;
        	}
        	if(startPoint == null) {
        		return;
        	}
            
    		endPoint = event.getPoint();
    		
    		absSelectionBox = getSelectionBox(startPoint, endPoint);
    		parent.setSelectionBox(absSelectionBox);
    		parent.deselect(selectedEntities);
    		selectedEntities = parent.getSelectedEntities();
    	}
    	
    	public Rectangle getSelectionBox(Point startPoint, Point endPoint) {
    		int startX = startPoint.x;
    		int startY = startPoint.y;
    		
    		int endX = endPoint.x;
    		int endY = endPoint.y;
    		
    		int width = endX - startX;
    		int height = endY - startY;
    		
    		if(width == 0) {
    			return null;
    		}
    		if(height == 0) {
    			return null;
    		}
    		
    		int cornerX = startX;
    		int cornerY = startY;
    		
    		if(width < 0) {
    			cornerX = endX;
    			width = Math.abs(width);
    		}
    		
    		if(height < 0) {
    			cornerY = endY;
    			height = Math.abs(height);
    		}
    		
    		Rectangle selectionBox = new Rectangle(cornerX, cornerY, width, height);
    		
    		return selectionBox;
    	}
    }
}
