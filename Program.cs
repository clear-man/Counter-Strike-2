using CounterStrike2;
using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;

//Inicia o forms do namespace Loader para verificar a chave de licensa
Loader.Form1 form = new Loader.Form1();
form.ShowDialog();
//se o Form1 for fechado, o programa deve ser encerrado, mas se o Button1 (Login with license key) foi clicado e nao chamou Enviroment.Exit(0), o programa deve continuar
if(form.DialogResult == DialogResult.Cancel){
    Environment.Exit(0);
}


// Inicia cs2
Swed swed = new Swed("cs2");

// Pega endereço base do client.dll
IntPtr client = swed.GetModuleBase("client.dll");

Reader reader = new Reader(swed);

// Inicia renderer
Renderer renderer = new Renderer();
Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
renderThread.Start();

// Pega tamanho da tela do renderer
Vector2 screenSize = renderer.screenSize;
Vector2 windowLocation = new Vector2(0, 0);
Vector2 windowSize = new Vector2(1920, 1080);
Vector2 lineOrigin = new Vector2(1920/2, 1080);
Vector2 windowCenter = new Vector2(1920/2, 1080/2);

bool bombPlanted = false;

// Armazena entidades
List<Entity> entities = new List<Entity>();
//List<Entity> entitiesAimbot = new List<Entity>();
Entity localPlayer = new Entity();

// Loop principal
while (renderer.Running)
{
    entities.Clear(); // limpa lista de entidades
    //entitiesAimbot.Clear();

    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList); // pega lista de entidades

    IntPtr listEntry = swed.ReadPointer(entityList, 0x10); // começa a ler a lista de entidades

    IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn); // pega o localPlayerPawn
    
    IntPtr gameRules = swed.ReadPointer(client, Offsets.dwGameRules); // pega o gameRules

    localPlayer.team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum); // define o time do localPlayer

    localPlayer.health = swed.ReadInt(localPlayerPawn, Offsets.m_iHealth); // pega a vida do localPlayer

    localPlayer.viewOffset = swed.ReadVec(localPlayerPawn, Offsets.m_vecViewOffset); // pega o viewOffset do localPlayer
    
    localPlayer.position = swed.ReadVec(localPlayerPawn, Offsets.m_vOldOrigin); // pega a posição do localPlayer

    if(gameRules != IntPtr.Zero){
        bombPlanted = swed.ReadBool(gameRules, Offsets.m_bBombPlanted);
        if(bombPlanted && renderer.aux == true){
            _ = bombTimer();
            renderer.aux = false;
        }
    }

    //Console.Clear();
    entities.Clear();
    //entitiesAimbot.Clear();

    // Loop através da lista de entidades
    for (int i = 0; i < 64; i++)
    {

        if (listEntry == IntPtr.Zero)
        {
            continue;
        }

        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78); // pega o controlador da entidade

        if (currentController == IntPtr.Zero)
        {
            continue;
        }

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn); // pega o handle do pawn
        if (pawnHandle == 0)
        {
            continue;
        }

        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10); // pega a segunda entrada da lista de entidades (pawn atual)
        if (listEntry2 == IntPtr.Zero)
        {
            continue;
        }

        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF)); // pega o pawn atual
        if (currentPawn == IntPtr.Zero)
        {
            continue;
        }

        if(currentPawn == localPlayerPawn){
            continue;
        }

        // Checa lifeState
        uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);
        if (lifeState != 256)
        {
            continue;
        }

        int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        if(team == localPlayer.team && (renderer.apenasInimigos == true)){
            continue;
        }

        // Get viewMatrix
        IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);
        IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80);
        float[] viewmatrix = swed.ReadMatrix(client + Offsets.dwViewMatrix);
        viewMatrix viewmatrix2 = reader.ReadMatrix(client + Offsets.dwViewMatrix);

        // Popula Entity
        //Console.WriteLine("Populando entidade");
        Entity entity = new Entity();
        entity.pawnAddress = currentPawn;
        entity.controllerAddress = currentController;
        entity.lifeState = lifeState;
        //entity.spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);
        entity.team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        entity.health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
        entity.position = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.distance = Vector3.Distance(entity.position, localPlayer.position);
        entity.bones = reader.ReadBones(boneMatrix);
        entity.bones2D = reader.ReadBones2d(entity.bones, viewmatrix2, screenSize);
        //entity.viewOffset = swed.ReadVec(currentPawn, m_vecViewOffset);
        entity.viewOffset = new Vector3(0, 0, 65);
        entity.position2D = Calculate.WorldToScreen(viewmatrix, entity.position, screenSize);
        entity.viewPosition2D = Calculate.WorldToScreen(viewmatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);
        entity.head = swed.ReadVec(boneMatrix, 6*32);
        entity.head2d = Calculate.WorldToScreen2(viewmatrix2, entity.head, screenSize);
        entity.pixelDistance = Vector2.Distance(entity.head2d, windowCenter);
        //entity.abs = Vector3.Add(entity.position, entity.viewOffset);
        //entity.originScreenPosition = Vector2.Add(Calculate.WorldToScreen2(viewmatrix2, entity.position, windowSize), windowLocation);
        //entity.absScreenPosition = Vector2.Add(Calculate.WorldToScreen2(viewmatrix2, entity.abs, windowSize), windowLocation);
        //entity.angleDifference = Calculate.CalculatePixelDistance(windowCenter, entity.absScreenPosition);
        //entity.magnitude = Calculate.CalculateMagnitude(localPlayer.position, entity.position);

        entities.Add(entity);
        /* if(entity.position2D.X > 0 && entity.position2D.X < screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < screenSize.Y){
            entitiesAimbot.Add(entity);
        } */
    }

    //entitiesAimbot = entitiesAimbot.OrderBy(o => o.angleDifference).ToList();
    //entities = entities.OrderBy(o => o.magnitude).ToList();
    entities = entities.OrderBy(o => o.pixelDistance).ToList();

    if(entities.Count > 0 && renderer.aimbot && renderer.ativarAimbot){
        Vector3 playerView = Vector3.Add(localPlayer.position, localPlayer.viewOffset);
        Vector3 entityView = Vector3.Add(entities[0].position, entities[0].viewOffset);

        if(entities[0].pixelDistance < renderer.fov){
            Vector2 newAngles = Calculate.CalculateAngles(playerView, entities[0].head);
            Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);
            swed.WriteVec(client, Offsets.dwViewAngles, newAnglesVec3);
        }
    }

    // Atualiza os dados renderizados
    renderer.UpdateLocalPlayer(localPlayer);
    renderer.UpdateEntities(entities);

     //Thread.Sleep(14); // se necessário

    if(GetAsyncKeyState(renderer.selectedKey) < 0){
        renderer.aimbot = !renderer.aimbot;
        Thread.Sleep(100);
    }
}
[DllImport("user32.dll")]
     static extern short GetAsyncKeyState(int vKey);

async Task bombTimer(){
    IntPtr gameRules = swed.ReadPointer(client, Offsets.dwGameRules);
    for(int i=0 ; i<=40 ; i++){
                bool bombPlanted = swed.ReadBool(gameRules, Offsets.m_bBombPlanted);
                if(!bombPlanted){
                    break;
                }
                int timeLeft = 40 - i;
                renderer.timeLeft = timeLeft;
                renderer.bombPlanted = true;
                await Task.Delay(1000);
            }
        renderer.bombPlanted = false;
        renderer.timeLeft = -1;
        renderer.aux = true;
}
