using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class NoiseNode : NoiseNodeBase
    {
        [Header("Seeding")] public int  seed;
        public                     bool useRandomSeed;

        public FastNoiseLite.NoiseType noiseType;
        public float                   frequency;

        public FastNoiseLite.FractalType fractalType;
        public int                       octaves;
        public float                     impact = .5f;

        private FastNoiseLite gen;

        public override void Prepare(int seed2)
        {
            gen ??= new FastNoiseLite();

            gen.SetSeed(seed + seed2);
            gen.SetFrequency(frequency);
            gen.SetNoiseType(noiseType);
            gen.SetFractalOctaves(octaves);
            gen.SetFractalGain(impact);
            gen.SetFractalLacunarity(1 / impact);
            gen.SetFractalType(fractalType);
        }

        public override float Sample(Vector3 pos)
        {
            float noise = gen.GetNoise(pos.x, pos.y, pos.z);
            noise = (noise + 1) / 2;
            return noise;
        }
    }
}