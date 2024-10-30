namespace CounterStrike2
{
    public static class Offsets{
        // OFFSETS
        public static int dwEntityList = 0x19CFC48;
        public static int dwViewMatrix = 0x1A31D30;
        public static int dwLocalPlayerPawn = 0x1834B18;
        public static int dwViewAngles = 0x1A3BBB0;
        public static int dwGameRules = 0x1A2D668;

        // CLIENT.DLL OFFSETS
        public static int m_vOldOrigin = 0x1324;
        public static int m_iTeamNum = 0x3E3;
        public static int m_lifeState = 0x348;
        public static int m_hPlayerPawn = 0x80C;
        public static int m_vecViewOffset = 0xCB0;
        public static int m_iHealth = 0x344;
        public static int m_pGameSceneNode = 0x328;
        public static int m_modelState = 0x170;
        public static int m_entitySpottedState = 0x23B8;
        public static int m_bSpotted = 0x8;
        public static int m_bBombPlanted = 0x9A5;
    }
}