var terrain = document.getElementById("terrain");
var callqueue = [];
var isgenerate = false;
var checkgenerate = false;
var isLock = false;
var xShift = [-1,0,1,-1,1,-1,0,1];
var yShift = [-1,-1,-1,0,0,1,1,1];
var xSide = [0,0,-1,1];
var ySide = [-1,1,0,0];
var rows = 300;
var cols = 300;
var tileCount = 0;
var seaCount = 0;

function setTile(x,y,type)
{
    if(emptyTile(x,y))
    {
        tileCount++; 
    }
    terrain.rows[x].cells[y].className = type;
    callqueue.push( "processTile("+x+","+y+")" );
    //isterrain[type] = true;
}
function generateTile(x,y,type)
{
    //if(Math.floor(Math.random()*100000) < 1)
    //{
    //    isgenerate = true;
    //}
    setTile(x,y,type);
}
function emptyTile(x,y)
{
    return terrain.rows[x].cells[y].className == "";
}
function generateSea(x,y)
{
    if (x>-1 && x<rows && y>-1 && y<cols && terrain.rows[x].cells[y].className == "")
    {
        seaCount++;
        terrain.rows[x].cells[y].className = "sea";
        callqueue.push( "processSea("+x+","+y+")" );
    }
}

function startGenerate()
{
    terrain.className = "terrain";
    for (var r=0;r<rows;++r)
    {
        var tr = terrain.appendChild(document.createElement('tr'));
        for (var c=0;c<cols;++c)
        {
            var cell = tr.appendChild(document.createElement('td'));
        }
    }
    var startTerrain = ["forest","desert","cold"];
    setTile(150,150,"forest");
    //callqueue.push("");
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
        while(callqueue.length > 0 && tileCount < 30000/* && !checkgenerate*/)
        {
        //while(callqueue[0] != "" && callqueue.length > 0)
        //{
        eval(callqueue[0]);
        callqueue.splice(0,1);
        //}
        //checkgenerate = isgenerate;
        //callqueue.splice(0,1);
        //callqueue.push("");
        }
        //if(checkgenerate)
        //{
        /*var fillSea;
        for (var x=1;x<rows-1;++x)
        {
            for (var y=1;y<cols-1;++y)
            {
                fillSea = true;
                for(var i=0;i<xSide.length;i++)
                {
                    if(emptyTile(x+xSide[i],y+ySide[i]))
                    {
                        fillSea = false;
                        break;
                    }
                }
                if(fillSea)
                {
                    terrain.rows[x].cells[y].className = "forest";
                }
            }
        }*/
        terrain.rows[0].cells[0].className = "sea";
        callqueue.push( "processSea(0,0)" );
        while(callqueue.length > 0)
        {
            eval(callqueue[0]);
            callqueue.splice(0,1);
        }
        terrain.className = "terrain filledsea";
        //}
        isLock = false;
        window.alert("Total land count: "+(rows*cols-seaCount)+"\nLand count: "+tileCount+"\nFilled land count: "+(rows*cols-tileCount-seaCount));
    }
}

function processTile(x,y)
{
    var type = "forest";//terrain.rows[x].cells[y].className;
    if(x < 1 || x > rows-2 || y < 1 || y > cols-2)
    {
        return;
    }
    /*else if(type == "forest" && isterrain["cold"] && isterrain["desert"])
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
    }*/
    
    var excludeCount = 7;
    var remove;
    for(var r=0;r<excludeCount;r++)
    {
        remove = Math.floor(Math.random()*xShift.length);
        xShift.splice(remove,1);
        yShift.splice(remove,1);
    }
    for(var i=0;i<xShift.length;i++)
    {
        //if(emptyTile(x+xShift[i],y+yShift[i]))
        //{
            //var i = Math.floor(Math.random()*xShift.length);
            generateTile(x+xShift[i],y+yShift[i],type);
        //}
    }
    xShift = [-1,0,1,-1,1,-1,0,1];
    yShift = [-1,-1,-1,0,0,1,1,1];
    /*for(var i=0;i<xShift.length;i++)
    {
        if(emptyTile(x+xShift[i],y+yShift[i]))
        {
            callqueue.push( "processTile("+x+","+y+")" );
            return;
        }
    }*/
}

function processSea(x,y)
{
    for(var i=0;i<xSide.length;i++)
    {
        generateSea(x+xSide[i],y+ySide[i])
    }
}
