#include <AccelStepper.h>
//#include <MultiStepper.h>

#define DISPLAY_TIMEOUT -1
//#define DEBUG
//#define CALIB_EN
/******************************************************************
  Created with PROGRAMINO IDE for Arduino - 12.02.2017 11:28:10
  Project     :
  Libraries   :
  Author      :
  Description :
******************************************************************/
#define MAX_BUFFER_SIZE 200
#define SERIAL_BUFFER 500
#define BUFFER 10
#define COORD_BUFFER MAX_BUFFER_SIZE
#define DATA_SIZE 5
#define DEFAULT_SPEED 180 // 200 was initial // 800 is test speed
#define DEFAULT_ACCEL 600 // 400 was initial
#define CALIB_SPEED_SLOW 1200 // 100 was smoother. 150 is too noisy.
#define CALIB_SPEED_FAST 600
#define WRIST_TRAVEL 380 // old val 3000 (380)
#define WRIST_SPEED 1400
#define WRIST_ACCEL 2000


String inputString = "";
String tempString = "";
boolean stringComplete = false;

unsigned int coord_size = COORD_BUFFER;
int coord_current = -1;
int coord_last = -1;

unsigned long timeout_t1 = 0;



unsigned short current_state = 0; // 0: idle, 1: moving motor single point, 2: calibration, 3: filling buffer, 4: stream mode

AccelStepper shoulder(AccelStepper::DRIVER, 11, 10) ; // Defaults to AccelStepper::FULL4WIRE (4 pins) on 2, 3, 4, 5
AccelStepper xaxis(AccelStepper::DRIVER, 7, 6) ; // Defaults to AccelStepper::FULL4WIRE (4 pins) on 2, 3, 4, 5
AccelStepper elbow(AccelStepper::DRIVER, 9, 8);
AccelStepper wrist(AccelStepper::DRIVER, A2, A3);


int emax = A4;
int emin = A5;
int smax = 2;
int smin = 3;
int xmax = 4;
int xmin = 5;

int wEn = A0;

char cBuffer[BUFFER];
long xVal[COORD_BUFFER];
long eVal[COORD_BUFFER];
long sVal[COORD_BUFFER];
boolean wVal[COORD_BUFFER];
long xMaxCoord = 2500;
long sMaxCoord = 4500;
long eMaxCoord = 7500; 
long wMaxCoord = 350;

int wState = 0;

float xSpeed = 0;
float sSpeed = 0;
float eSpeed = 0;
float wSpeed = 0;

float xAccel = 0;
float sAccel = 0;
float eAccel = 0;
float wAccel = 0;

long tCounter = 0;
int LED = 13;

void setup()
{ 
  initParam();
  #ifdef DEBUG
   Serial.println("parameters initialized");
   #endif
  initMotorParam();
  #ifdef DEBUG
  Serial.println("Motor initialized");
  #endif
  
  #ifdef DEBUG
    Serial.println("Completed init.");
  #endif  
  Serial.println('0'); // reply to proc: completed init and idle state
}

void loop()
{
 
  checkSerial();
 
  procSerial();
  
  runMotors();
 
  checkPosition();

  //checkLimits();
  if((millis() - timeout_t1) > DISPLAY_TIMEOUT && DISPLAY_TIMEOUT != -1) {
    #ifdef DEBUG
    Serial.println("Time out condition");
    #endif
    timeout_t1 = millis();
    timeoutDisplay();
  }
}

void calibLimits() {
 // wristOff();
  
  
  xaxis.setSpeed(-CALIB_SPEED_SLOW);
  while(digitalRead(xmin) == true) {
    xaxis.runSpeed();
  }
  xaxis.setCurrentPosition(0);
  xaxis.setSpeed(0);
  delay(1000);

   #ifdef DEBUG
  Serial.println("Xmin reached");
  #endif
  
  
  
  xaxis.setSpeed(CALIB_SPEED_SLOW);
  /*while(digitalRead(xmax) == true) {
    xaxis.runSpeed();
  }
  xMaxCoord = xaxis.currentPosition(); 
  xaxis.setSpeed(0);
  #ifdef DEBUG
  Serial.print("xmax: "); Serial.println(xMaxCoord);
  #endif
 delay(1000);
  */

/*
  xaxis.moveTo(0);
  while(xaxis.distanceToGo() !=0 ) {
    xaxis.run();
  }*/

  
  shoulder.setSpeed(-CALIB_SPEED_SLOW);
  while(digitalRead(smin) == true) {
    shoulder.runSpeed();
  }
  #ifdef DEBUG
  Serial.println("smin reached");
  #endif
  shoulder.setCurrentPosition(0);
  shoulder.setSpeed(0);
  delay(1000);
  
  shoulder.setSpeed(CALIB_SPEED_SLOW);
/*  while(digitalRead(smax) == true) {
    shoulder.runSpeed();
  }
  
  sMaxCoord = shoulder.currentPosition();
  #ifdef DEBUG
  Serial.print("Smax: "); Serial.println(sMaxCoord);
  #endif
  shoulder.setSpeed(0);
  delay(1000);
  */
  
  elbow.setSpeed(-CALIB_SPEED_SLOW);
  while(digitalRead(emin) == true) {
    elbow.runSpeed();
  }
  elbow.setCurrentPosition(0);
  elbow.setSpeed(0);
  #ifdef DEBUG
  Serial.println("emin reached");
  #endif
  
  delay(1000);  
  elbow.setSpeed(CALIB_SPEED_SLOW);
 /* while(digitalRead(emax) == true) {
    elbow.runSpeed();
  }
  eMaxCoord = elbow.currentPosition();
  #ifdef DEBUG
  Serial.print("Emax: "); Serial.println(eMaxCoord);
  #endif
  elbow.setSpeed(0);
delay(1000);
  */
  xaxis.setSpeed(DEFAULT_SPEED); xaxis.setMaxSpeed(DEFAULT_SPEED);
  shoulder.setSpeed(DEFAULT_SPEED); shoulder.setMaxSpeed(DEFAULT_SPEED);
  elbow.setSpeed(DEFAULT_SPEED); elbow.setMaxSpeed(DEFAULT_SPEED);

  #ifdef DEBUG
  Serial.println("Calibration complete. Going to xmin smax emax");
  #endif
  //xaxis.moveTo(0);
  //shoulder.moveTo(sMaxCoord);
  //elbow.moveTo(eMaxCoord);
  xaxis.moveTo(100);
  shoulder.moveTo(100);
  elbow.moveTo(100);
  calcMotorSpeed();
  Serial.println('c'); // reply to proc calibration complete
}

void wristOff() {
  digitalWrite(wEn, LOW);
  #ifdef DEBUG
  Serial.println("Switching wrist off");
  #endif
  
  if(wState != 0) {
  wrist.moveTo(-WRIST_TRAVEL);
  while(wrist.distanceToGo() != 0) {
    wrist.run();
  }
  wState = 0;
  }

  #ifdef DEBUG
  Serial.println("Wrist off");
  #endif
  digitalWrite(wEn, HIGH);
}

void wristOn() {
  digitalWrite(wEn, LOW);
  #ifdef DEBUG
  Serial.println("Switching wrist on");
  #endif
  
  if(wState != 1) {
   wrist.moveTo(0);
   while(wrist.distanceToGo() != 0) {
     wrist.run();
   }
   wState = 1;
  }
  #ifdef DEBUG
  Serial.println("Wrist on");
  #endif
 // digitalWrite(wEn, HIGH);
}

void checkLimits() {
  if(digitalRead(xmin) == false) {
    #ifdef DEBUG
//    Serial.println("xmin");
    #endif
    if(xaxis.distanceToGo() < 0) {
     // xaxis.move(0);
     xaxis.stop();
    }
  }
  else if(digitalRead(xmax) == false) {
    #ifdef DEBUG
//    Serial.println("xmax");
    #endif
    
    if(xaxis.distanceToGo() > 0) {
     // xaxis.move(0);
     xaxis.stop();
    }
  }

  if(digitalRead(smin) == false) {
    #ifdef DEBUG
//    Serial.println("smin");
    #endif
    
    if(shoulder.distanceToGo() < 0) {
      //shoulder.move(0);
      shoulder.stop();
    }
  }
  else if(digitalRead(smax) == false) {
    #ifdef DEBUG
//    Serial.println("smax");
    #endif
    
    if(shoulder.distanceToGo() > 0) {
      //shoulder.move(0);
      shoulder.stop();
    }
  }

  if(digitalRead(emin) == false) {
    #ifdef DEBUG
  //  Serial.println("emin");
    #endif
    
    if(elbow.distanceToGo() < 0) {
    //  elbow.move(0);
    elbow.stop();
    }
  }
  else if(digitalRead(emax) == false) {
    #ifdef DEBUG
 //   Serial.println("emax");
    #endif
    
    if(elbow.distanceToGo() > 0) {
      //elbow.move(0);
      elbow.stop();
    }
  }
}

void timeoutDisplay() {
  //#ifdef DEBUG
  Serial.println("Timeout display.");
  Serial.print("Run mode: ");
  Serial.println(current_state);
  Serial.print("X: ");
  Serial.print(xaxis.currentPosition());
  Serial.print(" ( ");
  Serial.print(xaxis.distanceToGo());
  Serial.print(" ) Speed: ");
  Serial.println(xaxis.speed());
  Serial.print("S: ");
 Serial.print(shoulder.currentPosition());
  Serial.print(" ( ");
  Serial.print(shoulder.distanceToGo());
  Serial.print(" ) Speed: ");
  Serial.println(shoulder.speed());
  Serial.print("E: ");
  Serial.print(elbow.currentPosition());
  Serial.print(" ( ");
  Serial.print(elbow.distanceToGo());
  Serial.print(" ) Speed:");
  Serial.println(elbow.speed());
  Serial.print("W: ");
  Serial.print(wrist.currentPosition());
  Serial.print(" ( ");
  Serial.print(wrist.distanceToGo());
  Serial.print(" ) Speed: ");
  Serial.println(wrist.speed());
  //#endif
}

void checkPosition() {
  
 if(xaxis.distanceToGo() == 0 && elbow.distanceToGo() == 0 && shoulder.distanceToGo() == 0 && current_state != 0) { 
  #ifdef DEBUG 
  Serial.println("Destination reached");
  #endif
  
  if(coord_current == coord_size-1 /*coord_last*/ && current_state == 4) {
   updateRunMode(0);
   coord_last = -1; coord_current = -1;
   Serial.println('r'); // reply to proc sequence completed going to idle
   
  }   
  else if(current_state == 4) {
   coord_current++;
  /* if(coord_current > coord_size-1) {
    coord_current = 0;
    updateRunMode(0);
   }*/
   updateMotorsCoord(coord_current);
 //  checkCoordBuffer();
  }
  if(current_state == 1) {
    updateRunMode(0);
    Serial.println('m'); // reply to proc move complete
  }  
 } 
}

void updateRunMode(short mode) {
  #ifdef DEBUG
  Serial.print("Run mode set: ");
  Serial.println(mode);  
  #endif
  current_state = mode;
  coord_current = -1; coord_last = -1;
}

void initParam() {
 coord_size = COORD_BUFFER;
 coord_current = -1;
 coord_last = -1;
 xSpeed = DEFAULT_SPEED;
 sSpeed = DEFAULT_SPEED;
 eSpeed = DEFAULT_SPEED;
 wSpeed = WRIST_SPEED;
 xAccel = DEFAULT_ACCEL;
 sAccel = DEFAULT_ACCEL;
 eAccel = DEFAULT_ACCEL;
 wAccel = WRIST_ACCEL;
 wState = -1; // initial undefined state.
 
 
 inputString.reserve(SERIAL_BUFFER);
 tempString.reserve(BUFFER);   
 pinMode(LED, OUTPUT);
 pinMode(wEn , OUTPUT);
 tCounter = millis();
 digitalWrite(wEn, HIGH);
 Serial.begin(115200);
 #ifdef DEBUG
 Serial.println("Serial started");
 #endif
}

void initMotorParam() {  
  xaxis.setMaxSpeed(xSpeed);
  xaxis.setSpeed(xSpeed);
  xaxis.setAcceleration(xAccel);
  shoulder.setMaxSpeed(sSpeed);
  shoulder.setSpeed(sSpeed);
  shoulder.setAcceleration(sAccel);
  elbow.setMaxSpeed(eSpeed);
  elbow.setSpeed(eSpeed);
  elbow.setAcceleration(eAccel); 
  wrist.setMaxSpeed(wSpeed);
  wrist.setSpeed(wSpeed);
  wrist.setAcceleration(wSpeed);

  //wristOff();
  #ifdef CALIB_EN
  calibLimits();
  #endif
}

void checkSerial() {
 if (Serial.available() && !stringComplete) { // default is while
    char inChar = (char)Serial.read();
    inputString += inChar;
    if (inChar == '\n') {
      stringComplete = true;
    }
    if (inputString.length() == (SERIAL_BUFFER - 1)) {
      inputString += '\n';
      stringComplete = true;
    }
  }
  else {
    if(millis() - tCounter > 10) {
 //     Serial.println("a");    
    }
  } 
}

void procSerial() {
  char cmd_rcvd;
  int cmd_enum = 0;
  if (stringComplete) {
    digitalWrite(LED, HIGH);
    #ifdef DEBUG
    Serial.print(inputString);
    Serial.println("Received");
    #endif
    cmd_rcvd = inputString[0];
  
    switch (cmd_rcvd) {
      case 'm': {
        #ifdef DEBUG
        Serial.println("move command");
        #endif
        getCoord(0);
        updateMotorsCoord(0);
        printTargetCoord(0);
        updateRunMode(1);  
        clearBuffer();     
        break;
      }
      case 'c': {
        #ifdef DEBUG
        Serial.println("calib command");
        #endif
        
        calibLimits();
        clearBuffer();
        break;
      }
      case 'p': {
        #ifdef DEBUG
        Serial.println("print coord command");
        #endif
        
        printCoord();
        clearBuffer();
        break;
      }

      case 'z': {
        #ifdef DEBUG
        Serial.println("print accel command");
        #endif
        
        printAccel();
        clearBuffer();
        break;
      }

      case 'x': {
        #ifdef DEBUG
        Serial.println("print speed command");
        #endif
        
        printSpeed();
        clearBuffer();
        break;
      }





      case 'u': {
        #ifdef DEBUG
        Serial.println("update coord command");
        #endif
        
        updatePosition();
        clearBuffer();
        break;
      }
      
      case 's': {
        #ifdef DEBUG
        Serial.println("speed command");
        #endif
        
        getSpeed();
        updateMotorSpeed();
        //printSpeed();
        clearBuffer();
        break;
      }
      case 'a': {
        #ifdef DEBUG
        Serial.println("accel command");
        #endif
        
        getAccel();
        updateMotorAccel();
        //printAccel();
        clearBuffer();
        break;
      }
      case 'q': {
        #ifdef DEBUG
        Serial.println("stream command");
        #endif
        
        getCoordStream();            
        #ifdef DEBUG        
        Serial.print("coord current: ") ; Serial.print(coord_current);
        Serial.print(". coord last: "); Serial.println(coord_last);
        #endif
        
        if((coord_current == -1 && coord_last >= coord_size-1)) {
          #ifdef DEBUG
          Serial.println("Starting stream processing");
          #endif
          updateRunMode(4);
          coord_current = 0;
          updateMotorsCoord(coord_current);
        }
        clearBuffer();
        break;
      }
      default: {
        clearBuffer();
        break;
      }
    }    
  }
}

void getCoordStream() {
  coord_last++;
  /*if(coord_last > coord_size-1) {
    coord_last = 0;
  }*/
  getCoord(coord_last);  
  checkCoordBuffer();
}

void checkCoordBuffer() {
  int coord_next = coord_last + 1;
 /* if (coord_next > coord_size -1) {
    coord_next = 0;
  }*/
  if(coord_next < coord_size) {
    Serial.println('q'); // reply to proc waiting for next coord
  }
  else {
    Serial.println('b'); // reply to proc buffer full. drawing
    #ifdef DEBUG
    Serial.println("buffer full");
    #endif
  }
}

void printTargetCoord(unsigned int run_index) {

  #ifdef DEBUG
        Serial.println("Received");
        Serial.println("Target coord");
        Serial.print("X: ");
        Serial.print(xVal[run_index]); Serial.print(" ( "); Serial.print(xaxis.currentPosition()); Serial.println(" ) ");
        Serial.print("S: ");
        Serial.print(sVal[run_index]); Serial.print(" ( "); Serial.print(shoulder.currentPosition()); Serial.println(" ) ");
        Serial.print("E: ");
        Serial.print(eVal[run_index]); Serial.print(" ( "); Serial.print(shoulder.currentPosition()); Serial.println(" ) ");
         Serial.print("W: ");
        Serial.println(wVal[run_index]);
  #endif
 
}

void printSpeed() {
  
        Serial.println("Speed");
        Serial.print("X: ");
        Serial.println(xSpeed);
        Serial.print("S: ");
        Serial.println(sSpeed);
        Serial.print("E: ");
        Serial.println(eSpeed);
        Serial.print("W: ");
        Serial.println(wSpeed);
  
}

void printAccel() {
  
        Serial.println("Accel");
        Serial.print("X: ");
        Serial.println(xAccel);
        Serial.print("S: ");
        Serial.println(sAccel);
        Serial.print("E: ");
        Serial.println(eAccel);
        Serial.print("W: ");
        Serial.println(wAccel);
}

void updateMotorsCoord(unsigned int run_index) {
 #ifdef DEBUG
 Serial.print("Processing index: " );
 Serial.println(run_index);
 #endif
 printTargetCoord(run_index);
 xaxis.moveTo(xVal[run_index]);
 shoulder.moveTo(sVal[run_index]);
 elbow.moveTo(eVal[run_index]);
 if(wVal[run_index] == 0) {
  wristOff();
 }
 else if(wVal[run_index] == 1) {
  wristOn();
 }
 calcMotorSpeed();
// checkCoordBuffer();
}

void calcMotorSpeed() {
  long xdist = xaxis.distanceToGo();
  long sdist = shoulder.distanceToGo();
  long edist = elbow.distanceToGo();
  if(xdist == 0) {
    xSpeed = 0;
  }
  else if(abs(xdist) >= abs(sdist) && abs(xdist) >= abs(edist)) {
    xSpeed = float(DEFAULT_SPEED);
    sSpeed = float(abs(xSpeed*sdist/xdist));
    eSpeed = float(abs(xSpeed*edist/xdist));
  }

  if(sdist == 0) {
    sSpeed = 0;
  }
  else if(abs(sdist) >= abs(xdist) && abs(sdist) >= abs(edist)) {
    sSpeed = float(DEFAULT_SPEED);
    xSpeed = float(abs(sSpeed*xdist/sdist));
    eSpeed = float(abs(sSpeed*edist/sdist));
  }
  if(edist == 0) {
    eSpeed = 0;
  }
  else if(abs(edist) >= abs(sdist) && abs(edist) >= abs(xdist)) {
    eSpeed = float(DEFAULT_SPEED);
    sSpeed = float(abs(eSpeed*sdist/edist));
    xSpeed = float(abs(eSpeed*xdist/edist));
  }
  updateMotorSpeed();
  calcMotorAcc();
}

void calcMotorAcc() {
  long xdist = xaxis.distanceToGo();
  long sdist = shoulder.distanceToGo();
  long edist = elbow.distanceToGo();
  float xSpeed = xaxis.maxSpeed();
  float sSpeed = shoulder.maxSpeed();
  float eSpeed = shoulder.maxSpeed();
  if(xSpeed == 0) {
    xAccel = 0;
  }
  else if(abs(xSpeed) >= abs(sSpeed) && abs(xSpeed) >= abs(eSpeed)) {
    xAccel = float(DEFAULT_ACCEL);
    sAccel = float(abs(xAccel*sSpeed/xSpeed));
    eAccel = float(abs(xAccel*eSpeed/xSpeed));
  }

  if(sSpeed == 0) {
    sAccel = 0;
  }
  else if(abs(sSpeed) >= abs(xSpeed) && abs(sSpeed) >= abs(eSpeed)) {
    sAccel = float(DEFAULT_ACCEL);
    xAccel = float(abs(sAccel*xSpeed/sSpeed));
    eAccel = float(abs(sAccel*eSpeed/sSpeed));
  }
  if(eSpeed == 0) {
    eAccel = 0;
  }
  else if(abs(eSpeed) >= abs(sSpeed) && abs(eSpeed) >= abs(xSpeed)) {
    eAccel = float(DEFAULT_ACCEL);
    sAccel = float(abs(eAccel*sSpeed/eSpeed));
    xAccel = float(abs(eAccel*xSpeed/eSpeed));
  }
  updateMotorAccel();
  
}

void updateMotorSpeed() {
 xaxis.setMaxSpeed(xSpeed);
 xaxis.setSpeed(xSpeed);
 shoulder.setMaxSpeed(sSpeed);
 shoulder.setSpeed(sSpeed);
 elbow.setMaxSpeed(eSpeed);
 elbow.setSpeed(eSpeed);
 //printSpeed();
}

void updateMotorAccel() {
 xaxis.setAcceleration(xAccel);
 shoulder.setAcceleration(sAccel);
 elbow.setAcceleration(eAccel);
}

void runMotors() {
  elbow.run();
  shoulder.run();
  xaxis.run(); 
}

void getCoord(unsigned int store_index) {
    tempString = inputString.substring(1,DATA_SIZE+1);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";
   
    xVal[store_index] = atol(cBuffer);
    #ifdef DEBUG
    Serial.print("x val: ");
    Serial.println(xVal[store_index]);
    #endif
    tempString = inputString.substring(DATA_SIZE+2,2*DATA_SIZE+2);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";    
 
    sVal[store_index] = atol(cBuffer);
    #ifdef DEBUG
    Serial.print("s val: ");
    Serial.println(sVal[store_index]);
    #endif
    
    tempString = inputString.substring(2*DATA_SIZE+3,3*DATA_SIZE+3);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";
  
    eVal[store_index] = atol(cBuffer);
    #ifdef DEBUG
    Serial.print("e val: ");
    Serial.println(eVal[store_index]);
    #endif
    
    tempString = inputString.substring(3*DATA_SIZE+4,3*DATA_SIZE+5);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";
 
    wVal[store_index] = atol(cBuffer);
  /*  #ifdef DEBUG
    Serial.print("w val: ");
    Serial.println(wVal[store_index]);
    #endif
    */
    clearBuffer();
    #ifdef DEBUG
    Serial.print("Received coord: ");
    Serial.println(store_index);
    #endif
}

void getSpeed() {
  tempString = inputString.substring(1,DATA_SIZE+1);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";
    xSpeed = atol(cBuffer);
    sSpeed = xSpeed;
    eSpeed = sSpeed;
    Serial.println('s'); //reply to proc speed accepted
    
    clearBuffer();
}

void getAccel() {
    tempString = inputString.substring(1,DATA_SIZE+1);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";
    xAccel = atol(cBuffer);
    sAccel = xAccel;
    eAccel = sAccel;
    Serial.println('a'); // reply to proc accel accepted
    clearBuffer();
}

void printCoord() {
  Serial.println(xaxis.currentPosition());
  Serial.println(shoulder.currentPosition());
  Serial.println(elbow.currentPosition());
}

void updatePosition() {
    tempString = inputString.substring(1,DATA_SIZE+1);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";
   
    xaxis.setCurrentPosition(atol(cBuffer));
  /*  #ifdef DEBUG
    Serial.print("x val: ");
    Serial.println(xVal[store_index]);
    #endif*/
    tempString = inputString.substring(DATA_SIZE+2,2*DATA_SIZE+2);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";    
 
    shoulder.setCurrentPosition(atol(cBuffer));
  /*  #ifdef DEBUG
    Serial.print("s val: ");
    Serial.println(sVal[store_index]);
    #endif
    */
    tempString = inputString.substring(2*DATA_SIZE+3,3*DATA_SIZE+3);
    tempString.toCharArray(cBuffer, BUFFER);
    tempString = "";
  
    elbow.setCurrentPosition(atol(cBuffer));
   /* #ifdef DEBUG
    Serial.print("e val: ");
    Serial.println(eVal[store_index]);
    #endif
    */
    
  /*  #ifdef DEBUG
    Serial.print("w val: ");
    Serial.println(wVal[store_index]);
    #endif
    */
    Serial.println('u'); // reply to proc update complete
    clearBuffer();
    #ifdef DEBUG
    Serial.print("Coord set ");
    Serial.print("X: "); Serial.println(xaxis.currentPosition());
    Serial.print("S: "); Serial.println(shoulder.currentPosition());
    Serial.print("E: "); Serial.println(elbow.currentPosition());
    #endif
}


void clearBuffer() {  
    stringComplete = false;
    inputString = "";
    tCounter = millis();
}
