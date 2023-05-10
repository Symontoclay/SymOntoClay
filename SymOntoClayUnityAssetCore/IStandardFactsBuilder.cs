/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IStandardFactsBuilder
    {
        /// <summary>
        /// Builds fact that the NPC says something.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <param name="factStr">Fact which is said by the NPC.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildSayFactString(string selfId, string factStr);

        /// <summary>
        /// Builds fact that the NPC says something.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <param name="fact">Fact which is said by the NPC.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildSayFactInstance(string selfId, RuleInstance fact);

        /// <summary>
        /// Builds fact which represents listening to a sound.
        /// </summary>
        /// <param name="distance">Distance to sound source.</param>
        /// <param name="directionToPosition">Direction to to sound source.</param>
        /// <param name="factStr">Fact which represents a sound.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildSoundFactString(double distance, float directionToPosition, string factStr);

        /// <summary>
        /// Builds fact which represents listening to a sound.
        /// </summary>
        /// <param name="distance">Distance to sound source.</param>
        /// <param name="directionToPosition">Direction to to sound source.</param>
        /// <param name="fact">Fact which represents a sound.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildSoundFactInstance(double distance, float directionToPosition, RuleInstance fact);

        /// <summary>
        /// Builds fact that the NPC is alive.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildAliveFactString(string selfId);

        /// <summary>
        /// Builds fact that the NPC is alive.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildAliveFactInstance(string selfId);

        /// <summary>
        /// Builds fact that the NPC is dead.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildDeadFactString(string selfId);

        /// <summary>
        /// Builds fact that the NPC is dead.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildDeadFactInstance(string selfId);

        /// <summary>
        /// Builds fact that the NPC has stopped itself.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildStopFactString(string selfId);

        /// <summary>
        /// Builds fact that the NPC has stopped itself.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildStopFactInstance(string selfId);

        /// <summary>
        /// Builds fact that the NPC has started walking.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildWalkFactString(string selfId);

        /// <summary>
        /// Builds fact that the NPC has started walking.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildWalkFactInstance(string selfId);

        /// <summary>
        /// Builds fact which represents sound of someone's walking.
        /// </summary>
        /// <returns>String that represents built fact.</returns>
        string BuildWalkSoundFactString();

        /// <summary>
        /// Builds fact which represents sound of someone's walking.
        /// </summary>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildWalkSoundFactInstance();

        /// <summary>
        /// Builds fact that the NPC has started running.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildRunFactString(string selfId);

        /// <summary>
        /// Builds fact that the NPC has started running.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildRunFactInstance(string selfId);

        /// <summary>
        /// Builds fact which represents sound of someone's running.
        /// </summary>
        /// <returns>String that represents built fact.</returns>
        string BuildRunSoundFactString();

        /// <summary>
        /// Builds fact which represents sound of someone's running.
        /// </summary>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildRunSoundFactInstance();

        /// <summary>
        /// Builds fact that the NPC holds something in his hands.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <param name="heldThingId">Id of held thing.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildHoldFactString(string selfId, string heldThingId);

        /// <summary>
        /// Builds fact that the NPC holds something in his hands.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <param name="heldThingId">Id of held thing.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildHoldFactInstance(string selfId, string heldThingId);

        /// <summary>
        /// Builds fact that the NPC shoots.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildShootFactString(string selfId);

        /// <summary>
        /// Builds fact that the NPC shoots.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildShootFactInstance(string selfId);

        /// <summary>
        /// Builds fact which represents sound of someone's shooting.
        /// </summary>
        /// <returns>String that represents built fact.</returns>
        string BuildShootSoundFactString();

        /// <summary>
        /// Builds fact which represents sound of someone's shooting.
        /// </summary>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildShootSoundFactInstance();

        /// <summary>
        /// Builds fact that the NPC is ready for shooting.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>String that represents built fact.</returns>
        string BuildReadyForShootFactString(string selfId);

        /// <summary>
        /// Builds fact that the NPC is ready for shooting.
        /// </summary>
        /// <param name="selfId">Id of the NPC.</param>
        /// <returns>Instance of built fact.</returns>
        RuleInstance BuildReadyForShootFactInstance(string selfId);

        string BuildSeeFactString(string seenObjId);
        string BuildFocusFactString(string seenObjId);
        string BuildDefaultInheritanceFactString(string obj, string superObj);
    }
}
