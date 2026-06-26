#include <AccelStepper.h>
#include <MultiStepper.h>
#include<math.h>

long int tomovesteps[3];
byte dir1pin=2;
byte step1Pin=3;     //motor 1

byte dir2pin=4; 
byte step2Pin=5;       //motor 2

byte dir3pin=6;   
byte step3Pin=7;        //motor 3

byte motor3ground=8;
//Limit switches
int pos1 = 53;      //LM 1
int sig1 = 51;

int pos2 = 47;      //LM 2
int neg2 = 45;
int sig2 = 43;

int pos3 = 39;      //LM 3
int neg3 = 37;
int sig3 = 35;

AccelStepper stepper1 (AccelStepper::DRIVER,step1Pin,dir1pin);
AccelStepper stepper2 (AccelStepper::DRIVER,step2Pin,dir2pin);
AccelStepper stepper3 (AccelStepper::DRIVER,step3Pin,dir3pin); 
MultiStepper Moveall;

double a,b,c;
void setup() {
Serial.begin(9600);
    pinMode(13,OUTPUT); 

    pinMode(step1Pin,OUTPUT); 
    pinMode(dir1pin,OUTPUT);

    pinMode(step2Pin,OUTPUT); 
    pinMode(dir2pin,OUTPUT);

    pinMode(step3Pin,OUTPUT); 
    pinMode(dir3pin,OUTPUT); 
    pinMode(motor3ground,OUTPUT); 


    stepper1.setMaxSpeed(800); 
    stepper2.setMaxSpeed(800);
    stepper3.setMaxSpeed(800);

    Moveall.addStepper(stepper1);
    Moveall.addStepper(stepper2);
    Moveall.addStepper(stepper3);
   
    digitalWrite(motor3ground,LOW);
  
  pinMode(pos1, OUTPUT);
  pinMode(sig1,INPUT);
  pinMode(pos2, OUTPUT);
  pinMode(neg2, OUTPUT);
  pinMode(sig2, INPUT);
  pinMode(pos3, OUTPUT);
  pinMode(neg3, OUTPUT);
  pinMode(sig3, INPUT);

  digitalWrite(pos1, HIGH);
  digitalWrite(pos2, HIGH);
  digitalWrite(neg2, LOW);
  digitalWrite(pos3, HIGH);
  digitalWrite(neg3, LOW);

digitalWrite(dir1pin,HIGH); //setting motor dir
digitalWrite(dir3pin,HIGH);
digitalWrite(dir2pin,HIGH);
digitalWrite(13,LOW);

delay(2000);
Home();
}

void loop() {
if (Serial.available()>=1){
  String coords=Serial.readStringUntil('\n');
  if(coords=="H"){
tomovesteps[0]=0;
tomovesteps[1]=0;
tomovesteps[2]=0;
Moveall.moveTo(tomovesteps);
Moveall.runSpeedToPosition();
  }else{
  int comma_1=coords.indexOf(',');
  int comma_2=coords.indexOf(',',comma_1+1);

  String theta1=coords.substring(0,comma_1)+"\0";
  String theta2=coords.substring(comma_1+1,comma_2);
  String theta3=coords.substring(comma_2+1);
  
  a=(theta1.toFloat()*100)/100.0;
  b=(theta2.toFloat()*100)/100.0;
  c=(theta3.toFloat()*100)/100.0;

if (isnan(a)!=1 && isnan(b)!=1  && isnan(c)!=1){
tomovesteps[0]=-a/0.1125;
tomovesteps[1]=-b/0.1125;
tomovesteps[2]=-c/0.1125;
  //  digitalWrite(13,HIGH);

Moveall.moveTo(tomovesteps);
Moveall.runSpeedToPosition();
  }else{
      //  digitalWrite(13,LOW);
      }
}}
}

void Home(){
while(digitalRead(sig1)==0 || digitalRead(sig2)==0 || digitalRead(sig3)==0){

  if(digitalRead(sig1)==0){
   digitalWrite(step1Pin,HIGH);
   delayMicroseconds(1000);
   digitalWrite(step1Pin,LOW);
   delayMicroseconds(1000);
      }
  if(digitalRead(sig2)==0){
   digitalWrite(step2Pin,HIGH);
   delayMicroseconds(1000);
   digitalWrite(step2Pin,LOW);
   delayMicroseconds(1000);
      }
  if(digitalRead(sig3)==0){
   digitalWrite(step3Pin,HIGH);
   delayMicroseconds(1000);
   digitalWrite(step3Pin,LOW);
   delayMicroseconds(1000);
      }
}
}