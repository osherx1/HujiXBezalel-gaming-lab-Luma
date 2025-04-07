# 🌞🌙 Sun and Moon Game

## Overview
### **Sun and Moon** is a competitive, two-player game where two characters—Sun and Moon—compete to score points by reaching the finish line across various floating clouds. The game emphasizes quick reflexes, strategic jumping, and playful rivalry between the two players.  

---

## 🎮 How To Play

### **Objective**
- Score points by reaching the finish line before your opponent. First to reach a set number of points wins.

### **Players**
- Two players - Sun (Player 1) and Moon (Player 2).

### **Controls**  
- **Move**: Use keyboard or controller to move left or right.  
- **Jump**: Press the designated jump button to leap across clouds.  
- **Attack**: Attack the opponent to prevent them from reaching the finish line.  

### **Scoring Points**  
- Each time a player reaches the finish line, they earn a point.  

### **Winning**  
- The first player to reach the specified number of points wins the game.  

---

## 🛠️ Structure of the Project

### **1. Player System**
- Controls player movement, jumping, attacking, and reactions.  
- Uses animations for expressing emotions like *happy*, *sad*, and *surprised*.  
- Player actions are handled through Unity’s `PlayerInputManager`.  

### **2. Board System**
- Manages the clouds and the finish line.  
- Dynamically adjusts positions of clouds.  
- Ensures smooth transitions when moving between clouds.  

### **3. UI System**
- Handles different game screens (`StartScreen`, `InstructionsScreen`).  
- Smooth fade transitions between screens.  
- Dynamic switching of scenes based on player actions.  

### **4. Animation System**
- Adds personality to characters through animations.  
- Emotions are triggered based on actions (scoring, dying, getting attacked).  
- Integrated with the player system for seamless experience.  

### **5. Audio System**
- Background music and sound effects.  
- Transitioning between different audio tracks smoothly.  
- Sound effects tied to gameplay events.  

### **6. Scene Transitions**
- Fade effects are used to transition between screens and levels.  
- Smooth transitions without a sudden black screen.  

---

## 📂 Project Files

- **PlayerController.cs** - Handles player input, movement, and animations.  
- **PlayerManager.cs** - Manages the players' setup and interactions.  
- **UIManager.cs** - Handles UI screens and transitions between scenes.  
- **SoundManager.cs** - Manages audio playback and transitions.  
- **Board.cs** - Controls the game board setup, including clouds and finish line.  
- **MonoSingleton.cs** - Ensures managers are accessible globally and only one instance exists.  

---






