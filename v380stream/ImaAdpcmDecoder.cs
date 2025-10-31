namespace v380stream;

/// <summary>
/// IMA ADPCM decoder ported from C++ adpcm_decoder function.
/// Decodes IMA ADPCM audio data to 16-bit PCM.
/// </summary>
internal static class ImaAdpcmDecoder
{
    // Step size table (IMA ADPCM standard)
    private static readonly int[] StepSizeTable = {
        7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 19, 21, 23, 25, 28, 31,
        34, 37, 41, 45, 50, 55, 60, 66, 73, 80, 88, 97, 107, 118, 130, 143,
        157, 173, 190, 209, 230, 253, 279, 307, 337, 371, 408, 449, 494, 544, 598, 658,
        724, 796, 876, 963, 1060, 1166, 1282, 1411, 1552, 1707, 1878, 2066, 2272, 2499, 2749, 3024,
        3327, 3660, 4026, 4428, 4871, 5358, 5894, 6484, 7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
        15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767
    };

    // Step index adjustment table (matching C++ dword_E6F0)
    // C++ uses: dword_E6F0[v12 + 25] where v12 is nibble (0-15)
    // The table has -1 at indices 0-24, then 2,4,6,8 at indices 25-28
    // So for nibble 0: index 25 = 2, nibble 1: index 26 = 4, etc.
    // Standard IMA ADPCM: -1,-1,-1,-1,2,4,6,8 for nibbles 0-7
    private static readonly int[] StepIndexAdjust = {
        -1, -1, -1, -1, 2, 4, 6, 8,
        -1, -1, -1, -1, 2, 4, 6, 8
    };

    /// <summary>
    /// Decodes IMA ADPCM data to 16-bit PCM.
    /// </summary>
    /// <param name="adpcmData">ADPCM data starting with predictor (bytes 0-1) and step index (byte 2), OR raw ADPCM nibbles if useExistingState=true</param>
    /// <param name="outputSamples">Number of output PCM samples to generate</param>
    /// <param name="useExistingState">If true, uses provided predictor/stepIndex instead of reading from data</param>
    /// <param name="predictor">Initial predictor value (required if useExistingState=true)</param>
    /// <param name="stepIndex">Initial step index value (required if useExistingState=true)</param>
    /// <param name="finalPredictor">Output: final predictor value after decoding</param>
    /// <param name="finalStepIndex">Output: final step index value after decoding</param>
    /// <param name="nibbleFlag">Input/Output: nibble position (0=need new byte/low nibble, 1=high nibble from currentByte)</param>
    /// <param name="currentByte">Input/Output: current byte being processed (for high nibble)</param>
    /// <returns>16-bit PCM samples (little-endian)</returns>
    public static byte[] Decode(byte[] adpcmData, int outputSamples, bool useExistingState, int predictor, int stepIndex, out int finalPredictor, out int finalStepIndex, ref int nibbleFlag, ref int currentByte)
    {
        // C++ decoder reads from a2 (the input buffer):
        // v7 = (a2[1] << 8) | *a2;  // predictor from bytes 0-1
        // v5 = a2[2];                // step index from byte 2
        // v22 = a2 + 4;              // ADPCM data starts at byte 4
        
        int adpcmOffset = 0;
        
        if (useExistingState)
        {
            // Use provided state, decode raw ADPCM data directly
            if (adpcmData.Length < 1 || outputSamples < 1)
            {
                finalPredictor = predictor;
                finalStepIndex = stepIndex;
                return Array.Empty<byte>();
            }
            // Predictor and stepIndex are provided, start decoding immediately
        }
        else
        {
            // Read predictor and step index from data (standard ADPCM block format)
            if (adpcmData.Length < 4 || outputSamples < 2)
            {
                finalPredictor = 0;
                finalStepIndex = 0;
                return Array.Empty<byte>();
            }
            
            // Read predictor (16-bit, little-endian) from bytes 0-1
            predictor = adpcmData[0] | (adpcmData[1] << 8);
            if (predictor > 32767) predictor -= 65536; // Sign extend

            // Read step index from byte 2
            stepIndex = adpcmData[2];
            // Clamp step index (C++: if (v5 < 0) v5 = 0; else if (v5 > 88) v5 = 88)
            if (stepIndex < 0) stepIndex = 0;
            if (stepIndex > 88) stepIndex = 88;

            // ADPCM data starts at byte 4
            adpcmOffset = 4;
            
            // Write first two samples from predictor
            // C++: *a3 = predictor; a3[1] = predictor;
        }

        // Allocate output buffer (16-bit samples = 2 bytes per sample)
        byte[] output = new byte[outputSamples * 2];
        int outputIndex = 0;

        // Write first two samples (from predictor) - C++ writes predictor twice
        // But only if NOT using existing state (new block starts with predictor)
        if (!useExistingState)
        {
            WriteSample(output, outputIndex, predictor);
            WriteSample(output, outputIndex + 2, predictor);
            outputIndex += 4;
        }

        // Get initial step size
        int stepSize = StepSizeTable[stepIndex];
        int sampleIndex = useExistingState ? 0 : 2; // Start at 0 if continuing, 2 if new block

        // Decode remaining samples (matching C++ implementation)
        // nibbleFlag and currentByte are passed in/out to maintain state across frames

        // Decode samples 2 through (outputSamples-1)
        // C++: while (v21 >= v26 - 1) break, where v21 starts at 0, v26 = outputSamples
        // So it decodes while sampleIndex < outputSamples - 1
        while (sampleIndex < outputSamples && adpcmOffset < adpcmData.Length)
        {
            int nibble;
            if (nibbleFlag == 0)
            {
                // Read new byte, use low nibble
                if (adpcmOffset >= adpcmData.Length) break;
                currentByte = adpcmData[adpcmOffset++];
                nibble = currentByte & 0x0F;
                nibbleFlag = 1;
            }
            else
            {
                // Use high nibble from previous byte
                nibble = (currentByte >> 4) & 0x0F;
                nibbleFlag = 0;
            }

            // Update step index (standard IMA ADPCM adjustment)
            // C++: dword_E6F0[v12 + 25] maps to standard table for nibbles 0-15
            stepIndex += StepIndexAdjust[nibble];
            
            if (stepIndex < 0) stepIndex = 0;
            if (stepIndex > 88) stepIndex = 88;

            // Get current step size
            stepSize = StepSizeTable[stepIndex];

            // Calculate sample delta (matching C++ logic)
            int delta = stepSize >> 3;  // v16 = v11 >> 3
            int nibbleLow = nibble & 7; // v15 = v12 & 7
            if ((nibbleLow & 4) != 0) delta += stepSize;      // if (v15 & 4) v16 += v11
            if ((nibbleLow & 2) != 0) delta += stepSize >> 1; // if (v15 & 2) v16 += v11 >> 1
            if ((nibbleLow & 1) != 0) delta += stepSize >> 2; // if (v15 & 1) v16 += v11 >> 2

            // Apply sign (C++: if (v14) v17 = v7 - v16; else v17 = v7 + v16)
            if ((nibble & 8) != 0)
                predictor -= delta;  // v14 = v12 & 8, subtract if sign bit set
            else
                predictor += delta;

            // Clamp to 16-bit signed range (C++: if (v17 < -32768) v7 = -32768; if (v7 > 0x7FFF) v7 = 0x7FFF)
            if (predictor < -32768) predictor = -32768;
            if (predictor > 32767) predictor = 32767;

            // Write sample
            WriteSample(output, outputIndex, predictor);
            outputIndex += 2;
            sampleIndex++;
        }

        finalPredictor = predictor;
        finalStepIndex = stepIndex;
        return output;
    }

    private static void WriteSample(byte[] buffer, int offset, int sample)
    {
        if (offset + 1 >= buffer.Length) return;
        buffer[offset] = (byte)(sample & 0xFF);
        buffer[offset + 1] = (byte)((sample >> 8) & 0xFF);
    }

    /// <summary>
    /// Estimates the number of PCM samples that will be generated from ADPCM data.
    /// For IMA ADPCM: 4:1 compression ratio, but first 4 bytes are header.
    /// </summary>
    public static int EstimateOutputSamples(int adpcmDataLength)
    {
        if (adpcmDataLength < 4) return 0;
        // Each byte after header produces 2 samples (high and low nibbles)
        int adpcmBytes = adpcmDataLength - 4;
        return 2 + (adpcmBytes * 2); // 2 initial samples + 2 per byte
    }
}

