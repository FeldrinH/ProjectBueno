var terrain = document.getElementById("terrain");
var callqueue = [];
var isterrain = {};
isterrain["forest"] = false;
isterrain["cold"] = false;
isterrain["desert"] = false;
var isgenerate = {}
isgenerate["cold"] = false;
isgenerate["desert"] = false;
isgenerate["forestdesert"] = false;
isgenerate["forestcold"] = false;
var checkgenerate = {}
checkgenerate["cold"] = false;
checkgenerate["desert"] = false;
checkgenerate["forestdesert"] = false;
checkgenerate["forestcold"] = false;
var isLock = false;
var xShift = [-1,0,1,-1,1,-1,0,1];
var yShift = [-1,-1,-1,0,0,1,1,1];

function setTile(x,y,type)
{
    terrain.rows[x].cells[y].className = type;
    isterrain[type] = true;
    callqueue.push( "processTile("+x+","+y+")" );
}
function generateTile(x,y,type)
{
    if(Math.floor(Math.random()*10000) == 0)
    {
        switch(type)
        {
            case "forest":
              if(!isterrain["desert"] && !isterrain["cold"])
              {
                  if(Math.random() < 0.5)
                  {
                      setTile(x,y,"cold");
                      isgenerate["cold"] = true;
                  }
                  else
                  {
                      setTile(x,y,"desert");
                      isgenerate["desert"] = true;
                  }
              }
              else if(isterrain["desert"])
              {
                  setTile(x,y,"cold");
                  isgenerate["cold"] = true;
              }
              else if(isterrain["cold"])
              {
                  setTile(x,y,"desert");
                  isgenerate["desert"] = true;
              }
              break;
            case "desert":
              if(!isterrain["forest"])
              {
                  setTile(x,y,"forest");
              }
              isgenerate["forestdesert"] = true;
              break;
            case "cold":
              if(!isterrain["forest"])
              {
                  setTile(x,y,"forest");
              }
              isgenerate["forestcold"] = true;
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
    return terrain.rows[x].cells[y].className == "";
}

function startGenerate()
{
    var rows = 500;
    var cols = 500;
    terrain.className = "terrain";
    for (var r=0;r<rows;++r)
    {
        var tr = terrain.appendChild(document.createElement('tr'));
        for (var c=0;c<cols;++c)
        {
            var cell = tr.appendChild(document.createElement('td'));
        }
    }
    setTile(250,250,"desert");
    callqueue.push("");
}

function generateNext(e)
{
    var unicode=e.keyCode ? e.keyCode : e.which
    if(unicode == 13 && !isLock)
    {
        isLock = true;
        if(callqueue.length == 0)
        {
            window.alert("CallQueue empty! (Should not happen)");
            return;
        }
        while(callqueue[0] != "" && callqueue.length > 0)
        {
            eval(callqueue[0]);
            callqueue.splice(0,1);
        }
        checkgenerate["cold"] = isgenerate["cold"];
        checkgenerate["desert"] = isgenerate["desert"];
        checkgenerate["forestdesert"] = isgenerate["forestdesert"];
        checkgenerate["forestcold"] = isgenerate["forestcold"];
        callqueue.splice(0,1);
        callqueue.push("");
        if(callqueue.length == 1)
        {
            terrain.className = "terrain sea";
        }
        isLock = false;
    }
}

function processTile(x,y)
{
    var type = terrain.rows[x].cells[y].className;
    if(x < 1 || x > 498 || y < 1 || y > 498)
    {
        return;
    }
    else if(type == "forest" && isterrain["cold"] && isterrain["desert"])
    {
        return;
    }
    else if(type == "forest" && ((isterrain["cold"] && !checkgenerate["forestcold"]) || (isterrain["desert"] && !checkgenerate["forestdesert"])))
    {
        callqueue.push( "processTile("+x+","+y+")" );
        return;
    }
    else if((type == "cold" && checkgenerate["forestcold"]) || (type == "desert" && checkgenerate["forestdesert"]))
    {
        return;
    }
    var exclude = Math.floor(Math.random()*8);
    for(var i=0;i<8;i++)
    {
        if(i != exclude && emptyTile(x+xShift[i],y+yShift[i]))
        {
            generateTile(x+xShift[i],y+yShift[i],type);
        }
    }
}
