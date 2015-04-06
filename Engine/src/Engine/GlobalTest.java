package Engine;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import org.junit.Before;
import org.junit.Test;

import static Engine.Global.*;
import static org.junit.Assert.*;

public class GlobalTest {
	List<String> listOfOptions = new ArrayList<>();
	String a = "a";
	String b = "b";
	String c = "c";
	String d = "d";
	String e = "e";
	String f = "f";

	@Before
	public void setUp() throws Exception {
		listOfOptions.add(a);
		listOfOptions.add(b);
		listOfOptions.add(c);
		listOfOptions.add(d);
		listOfOptions.add(e);
		listOfOptions.add(f);
	}

	@Test
	public void encodeDecode() {
		List<Integer> chosenOptions = new ArrayList<>();
		chosenOptions.add(0);
		chosenOptions.add(2);
		chosenOptions.add(3);
		chosenOptions.add(5);
		
		int encoded = encodeChoices(chosenOptions);
		assertTrue(encoded == 45);
		
		List<String> decoded = decodeChoices(encoded, listOfOptions);
		//Collections.reverse(decoded);
		String first = decoded.get(0);
		String second = decoded.get(1);
		String third = decoded.get(2);
		String forth = decoded.get(3);
		
		assertTrue(first == a);
		assertTrue(second == c);
		assertTrue(third == d);
		assertTrue(forth == f);
	}
}
