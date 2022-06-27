Shape={area=0}
function Shape:new(o,side)
	 o=o or {}
	 setmetatable(o,self)
	 self.__index=self
	 side=side or 0
     self.area=side*side
     return o
end

function  Shape:printArea()
	print("面积为",self.area)
end

myShape=Shape:new(nil,10)
myShape:printArea()

Square=Shape:new()
function Square:new(o,side)
	o=o or Shape:new(o,side)
	setmetatable(o,self)
	self.__index=self
	return o
end

function Square:printArea()
	print("正方形的面积为",self.area)
end

mySquare=Square:new(nil,22)
mySquare:printArea()

Rectangle=Shape:new()

function Rectangle:printArea()
	 print("矩形面积为",self.area)
end

myrectangle=Rectangle:new(nil,10,20)
myrectangle:printArea()