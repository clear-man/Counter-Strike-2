using System.Collections.Concurrent;
using System.Numerics;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace CounterStrike2
{
    
    public class Renderer : Overlay
{
    private bool running = true;
    public bool Running => running;

    //Renderiza variáveis
    public Vector2 screenSize = new Vector2(1920, 1080); //resolução da tela

    //copia entidades, utilizando ConcurrentQueue para evitar problemas de sincronização
    private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
    private Entity localPlayer = new Entity();
    private readonly object entityLock = new object();

    //GUI
    public bool ativar = false;
    public bool ativarAimbot = false;
    public bool apenasInimigos = true;
    private bool mostrarHP = false;
    private bool bone = false;
    private bool box = false;
    private bool line = false;
    public bool aimbot = false;
    public bool apenasVisivel = false;
    public float fov = 50;
    public bool mostrarFOV = false;
    private float boneThickness = 4;
    public int timeLeft = -1;
    public bool bombPlanted = false;
    public bool aux = true;
    public bool noRecoil = false;
    public bool c4Timer = false;
    private Vector4 enemyColor = new Vector4(1, 0, 0, 1); //vermelho
    private Vector4 teamColor = new Vector4(0, 1, 0, 1); //verde

    //keys (lista de botões para ativar o aimbot)
    private List<KeyValuePair<string, int>> keys = new List<KeyValuePair<string, int>>
    {
        new KeyValuePair<string, int>("Mouse1", 0x01),
        new KeyValuePair<string, int>("Mouse2", 0x02),
        new KeyValuePair<string, int>("Mouse3", 0x04),
        new KeyValuePair<string, int>("Mouse4", 0x05),
        new KeyValuePair<string, int>("Mouse5", 0x06),
    };

    public int selectedKey = 0x04;

    //lista para desenhar
    ImDrawListPtr drawList;

    protected override void Render()
    {
        // Alterar cores
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.TitleBg, new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.Tab, new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.TabHovered, new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.TabSelected, new Vector4(0.4f, 0.4f, 0.4f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.4f, 0.4f, 0.4f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(1f, 0.5f, 0.5f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.SliderGrab, new Vector4(1f, 0.5f, 0.5f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

        // Definir tamanho da janela
        ImGui.SetNextWindowSize(new Vector2(210, 240));
        ImGui.Begin("A Lenda - by clear-man", ImGuiWindowFlags.NoResize);

            // Definir estilo das abas
            if (ImGui.BeginTabBar("MenuTabs"))
            {
                if (ImGui.BeginTabItem("ESP"))
                {
                    CustomCheckbox("Ativar", ref ativar, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                    if(ativar){
                        CustomCheckbox("Apenas inimigos", ref apenasInimigos, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                        CustomCheckbox("Mostrar HP", ref mostrarHP, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                        CustomCheckbox("Mostrar esqueleto", ref bone, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                        CustomCheckbox("Mostrar caixa", ref box, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                        CustomCheckbox("Mostrar linha", ref line, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                    }
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Aimbot"))
                {
                    CustomCheckbox("Ativar", ref ativarAimbot, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                    if(ativarAimbot){
                        CustomCheckbox("Aimbot", ref aimbot, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(70);
                        ShowKeySelectionCombo(keys, ref selectedKey);
                        if(aimbot){
                            CustomCheckbox("Apenas inimigos", ref apenasInimigos, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                            //CustomCheckbox("Apenas visíveis", ref apenasVisivel, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                            ImGui.SliderFloat("FOV", ref fov, 1, 300);
                            CustomCheckbox("Mostrar FOV", ref mostrarFOV, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                        }
                    }
                    ImGui.EndTabItem();
                }

                /* if (ImGui.BeginTabItem("Outros"))
                {
                    CustomCheckbox("Ativar", ref ativar, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                    CustomCheckbox("Apenas inimigos", ref apenasInimigos, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                    ImGui.EndTabItem();
                } */

                if (ImGui.BeginTabItem("Outros"))
                {
                    CustomCheckbox("noRecoil", ref noRecoil, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                    CustomCheckbox("C4 Timer", ref c4Timer, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Sobre"))
                {
                    ImGui.Text("Criado por clear-man");
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.2f, 0.2f, 1.0f));
                    ImGui.TextWrapped("Todos os cheats que escrevem na memória, como aimbot, silentaim, norecoil, etc, são detectáveis e podem resultar em banimento.");
                    ImGui.TextWrapped("Cheats como ESP (e todas suas funcionalidades), C4 Timer, entre outros que apenas lêem a memória não são detectáveis pelo anti-cheat.");
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 1.0f));
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
        }

        float windowWidth = ImGui.GetWindowWidth();
        float windowHeight = ImGui.GetWindowHeight();
        float buttonWidth = ImGui.CalcTextSize("Encerrar").X + ImGui.GetStyle().FramePadding.X * 2.0f;
        float buttonHeight = ImGui.CalcTextSize("Encerrar").Y + ImGui.GetStyle().FramePadding.Y * 2.0f;
        ImGui.SetCursorPosX(windowWidth - buttonWidth - ImGui.GetStyle().WindowPadding.X);
        ImGui.SetCursorPosY(windowHeight - buttonHeight - ImGui.GetStyle().WindowPadding.Y);

        if (ImGui.Button("Encerrar"))
        {
            running = false;
            Environment.Exit(0);
        }

        ImGui.End();

        // Pop o estilo
        ImGui.PopStyleColor(9);

        // Desenha overlay
        DrawOverlay(screenSize);
        drawList = ImGui.GetWindowDrawList();

        if(c4Timer && (timeLeft >= 0 && timeLeft <= 40))
        drawList.AddText(null, 18.0f, new Vector2(screenSize.X / 2, screenSize.Y - (screenSize.Y - 100)), ImGui.ColorConvertFloat4ToU32(enemyColor), timeLeft.ToString());

        if (mostrarFOV && aimbot && ativarAimbot)
        drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), fov, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)));

        // draw stuff
        if (ativar)
        {
            foreach (var entity in entities)
            {
                // check if entity is on screen
                if (apenasInimigos == true)
                {
                    if (EntityOnScreen(entity) && entity.team != localPlayer.team) // desenha apenas inimigos
                    {
                        // draw methods
                        if(box){
                            DrawBox(entity);
                        }
                        if(line){
                            DrawLine(entity);
                        }
                        if(mostrarHP){
                            drawHealth(entity);
                        }
                        if(bone){
                            drawBone(entity);
                        }
                    }
                }
                else
                {
                    if (EntityOnScreen(entity)) // desenha também aliados
                    {
                        // draw methods
                        if(box){
                            DrawBox(entity);
                        }
                        if(line){
                            DrawLine(entity);
                        }
                        if(mostrarHP){
                            drawHealth(entity);
                        }
                        if(bone){
                            drawBone(entity);
                        }
                    }
                }
            }
        }
    }

    public void CustomCheckbox(string label, ref bool v, Vector4 bgColor)
        {
            ImGui.PushStyleColor(ImGuiCol.FrameBg, bgColor); // alterar a cor de fundo do checkbox
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, bgColor * 1.2f); // alterar a cor de fundo quando hover
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, bgColor * 1.5f); // alterar a cor de fundo quando ativo
            ImGui.Checkbox(label, ref v);
            ImGui.PopStyleColor();
        }

    public void ShowKeySelectionCombo(List<KeyValuePair<string, int>> keys, ref int selectedKey)
    {
        int aux = selectedKey;
        if (ImGui.BeginCombo(" ", keys.Find(x => x.Value == aux).Key))
        {
            foreach (var key in keys)
            {
                bool isSelected = (aux == key.Value);
                if (ImGui.Selectable(key.ToString(), isSelected))
                {
                    selectedKey = key.Value;
                }
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            } 
            ImGui.EndCombo();
        }
    }

    // check position
    bool EntityOnScreen(Entity entity)
    {
        if (entity.position2D.X > 0 && entity.position2D.X < screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < screenSize.Y)
        {
            return true;
        }
        return false;
    }

    // drawing methods

    private void drawHealth(Entity entity){
        Vector4 hpColor = localPlayer.team == entity.team ? teamColor : enemyColor;
        drawList.AddText(new Vector2(entity.viewPosition2D.X, entity.position2D.Y), ImGui.ColorConvertFloat4ToU32(hpColor), entity.health.ToString());
    }

    private void drawBone(Entity entity){
        if(entities.Count == 0 || entities == null){
            return;
        }

        List<Entity> tempEntities = new List<Entity>(entities).ToList();

        drawList = ImGui.GetWindowDrawList();
        Vector4 boneColor = localPlayer.team == entity.team ? teamColor : enemyColor;

        if(entity.bones2D[2].X > 0 && entity.bones2D[2].Y > 0 && entity.bones2D[2].X < screenSize.X && entity.bones2D[2].Y < screenSize.Y){
            float currentBoneThickness = boneThickness / entity.distance;
            drawList.AddLine(entity.bones2D[1], entity.bones2D[2], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[1], entity.bones2D[3], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[1], entity.bones2D[6], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[3], entity.bones2D[4], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[6], entity.bones2D[7], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[4], entity.bones2D[5], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[7], entity.bones2D[8], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[1], entity.bones2D[0], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[0], entity.bones2D[9], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[0], entity.bones2D[11], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[9], entity.bones2D[10], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddLine(entity.bones2D[11], entity.bones2D[12], ImGui.ColorConvertFloat4ToU32(boneColor), currentBoneThickness);
            drawList.AddCircle(entity.bones2D[2], 4 + currentBoneThickness, ImGui.ColorConvertFloat4ToU32(boneColor));
        }
    }

    private void DrawBox(Entity entity)
    {
        // calculate box height
        float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

        // calculate box dimensions
        Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);
        Vector2 rectBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

        // get correct color
        Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyColor;

        drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor));
    }

    private void DrawLine(Entity entity)
    {
        Vector4 lineColor = localPlayer.team == entity.team ? teamColor : enemyColor; // get correct color

        // draw line
        drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
    }

    // transfer entity methods
    public void UpdateEntities(IEnumerable<Entity> newEntities)
    { // update entities
        entities = new ConcurrentQueue<Entity>(newEntities);
    }

    public void UpdateLocalPlayer(Entity newEntity)
    { // update local player
        lock (entityLock)
        {
            localPlayer = newEntity;
        }
    }

    public Entity GetLocalPlayer()
    { // get local player
        lock (entityLock)
        {
            return localPlayer;
        }
    }

    void DrawOverlay(Vector2 screenSize)
    { // Overlay window
        ImGui.SetNextWindowSize(screenSize);
        ImGui.SetNextWindowPos(new Vector2(0, 0)); // in the beginning
        ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
    }
}

}