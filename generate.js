function new2Darray(rows) {
  var array = [];
  for (var i=0;i<rows;i++) {
     array[i] = [];
  }
  return array;
}

var terrain = document.getElementById("terrain");
var callqueue = [];
var isterrain = [];
isterrain["forest"] = false;
isterrain["cold"] = false;
isterrain["desert"] = false;
var checkterrain = []
checkterrain["forest"] = false;
checkterrain["cold"] = false;
checkterrain["desert"] = false;
var grid = new2Darray(100);
var xShift = [-1,0,1,-1,1,-1,0,1];
var yShift = [-1,-1,-1,0,0,1,1,1];

function setTile(x,y,type)
{
    grid[x][y] = type;
    terrain.rows[x].cells[y].className = type;
    isterrain[type] = true;
}
function generateTile(x,y,type)
{
    if(Math.floor(Math.random()*10000) == 0)
    {
        window.alert("Switch!");
        switch(type)
        {
            case "forest":
              window.alert("Place!");
              if(!checkterrain["desert"] && !checkterrain["cold"])
              {
                  if(Math.random() < 0.5)
                  {
                      setTile(x,y,"cold");
                  }
                  else
                  {
                      setTile(x,y,"desert");
                  }
              }
              else if(checkterrain["desert"])
              {
                  setTile(x,y,"cold");
              }
              else if(checkterrain["cold"])
              {
                  setTile(x,y,"desert");
              }
              break;
            case "desert":
              if(!checkterrain["forest"])
              {
                  setTile(x,y,"forest");
              }
              break;
            case "cold":
              if(!checkterrain["forest"])
              {
                  setTile(x,y,"forest");
              }
              break;
        }
    }
    else
    {
        setTile(x,y,type);
    }
}
function emptyTile(x,y)
{
    return !grid[x][y];
}

function startGenerate()
{
    var rows = 100
    var cols = 100
    terrain.className = 'terrain';
    for (var r=0;r<rows;++r)
    {
        var tr = terrain.appendChild(document.createElement('tr'));
        for (var c=0;c<cols;++c)
        {
            var cell = tr.appendChild(document.createElement('td'));
        }
    }
    setTile(50,50,"forest");
    callqueue.push("processTile(50,50)")
    callqueue.push("");
}

function generateNext(e)
{
    var unicode=e.keyCode ? e.keyCode : e.which
    if(unicode == 13)
    {
        //window.alert("Gen!")
        if(callqueue.length == 0)
        {
            window.alert("CallQueue empty! (Should not happen)");
            return;
        }
        while(callqueue[0] != "")
        {
            eval(callqueue[0]);
            callqueue.splice(0,1);
        }
        callqueue.splice(0,1);
        callqueue.push("");
        checkterrain["forest"] = isterrain["forest"];
        checkterrain["cold"] = isterrain["cold"];
        checkterrain["desert"] = isterrain["desert"];
    }
}

function processTile(x,y)
{
    if(x < 1 || x > 98 || y < 1 || y > 98)
    {
       return;
    }
    if(grid[x][y] == "forest" && (checkterrain["cold"] || checkterrain["desert"]))
    {
        return;
    }
    var exclude = Math.floor(Math.random()*8);
    for(var i=0;i<8;i++)
    {
        if(i != exclude && emptyTile(x+xShift[i],y+yShift[i]))
        {
            generateTile(x+xShift[i],y+yShift[i],grid[x][y]);
            callqueue.push( "processTile("+(x+xShift[i])+","+(y+yShift[i])+")" );
        }
    }
}